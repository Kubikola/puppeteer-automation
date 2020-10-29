﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Frontend.Forms;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;


namespace Frontend.UserControls
{
    public partial class EditUserControl : UserControl
    {
        private MainForm mf;
        private State recorderState = State.Disconnected;

      //  private Timer recordingTimer;
      private Task recordingTask;
      private Task replayTask;
      private CancellationTokenSource cts = new CancellationTokenSource();
      private CancellationToken cancelToken;

      public Process NodeJsProcess;

      private Filter activeFilter = null;
      private FilterForm filterForm = null;

      private ReplayViewForm replayViewForm;

      private ActionUserControl runningNowHighlightedAuc;
      private ActionUserControl oldErrorHighlightedAuc;

      private bool kill = false;

      private object colorChangeLck = new object();

      public void FilterChanged(Filter f)
      {
          if (f == null)
              return;

          activeFilter = f;

          if (f.EventTypes.Count == 0 && f.Status.Count == 0 && f.Targets.Count == 0)
          {
              filterLabel.Text = "Filter Disabled";
              filterLabel.ForeColor = Color.Red;
          }
          else
          {
              filterLabel.Text = "Filter Enabled";
              filterLabel.ForeColor = Color.ForestGreen;
          }
          ApplyFilter();
      }

      public void ApplyFilter()
      {
          if (activeFilter == null) 
              return;

          foreach (ActionUserControl auc in actionsFlowLayoutPanel.Controls)
          {
              dynamic action = auc.ExportActionForOutput();

              //event types
              if (activeFilter.EventTypes.FindIndex(x => NormalizeEventName(x) == action.type.ToString()) != -1)
              {
                  auc.Visible = false;
                  continue;
              }

              if (action.target == "selector" && activeFilter.Targets.Contains("Selector"))
              {
                  auc.Visible = false;
                  continue;
              }

              if (action.target != "selector" && activeFilter.Targets.Contains("Locator"))
              {
                  auc.Visible = false;
                  continue;
              }

              if (auc.EnabledForOutput && activeFilter.Status.Contains("Enabled"))
              {
                  auc.Visible = false;
                  continue;
              }

              if (!auc.EnabledForOutput && activeFilter.Status.Contains("Disabled"))
              {
                  auc.Visible = false;
                  continue;
              }

              if (auc.Selected && activeFilter.Status.Contains("Selected"))
              {
                  auc.Visible = false;
                  continue;
              }


              if (!auc.Selected && activeFilter.Status.Contains("Not Selected"))
              {
                  auc.Visible = false;
                  continue;
              }

              auc.Visible = true;
          }

          foreach (ActionUserControl auc in actionsFlowLayoutPanel.Controls)
              auc.UpdateUpDownButtons();
        }

      public State RecorderState
        {
            get { return recorderState; }
            set { 
                recorderState = value;
                UpdateUi();
            }
        }

      private bool SomeActionsSelectedForProcessing()
      {
          foreach (ActionUserControl auc in actionsFlowLayoutPanel.Controls)
          {
              if (enabledRadioButton.Checked && auc.EnabledForOutput)
                  return true;
              if (selectedRadioButton.Checked && auc.Selected)
                  return true;
          }

          return false;
      }

        private void UpdateUi()
        {
            if (recorderState == State.Connected)
            {
                browserConnection.Text = "Disconnect/Stop Browser";
                recordButton.Text = "Start Recordings";
                recordButton.Enabled = true;
                UpdateProcessButtons();
            }
            else if (recorderState == State.Disconnected)
            {
                browserConnection.Text = "Connect/Start Browser";
                recordButton.Enabled = false;
                optimizeButton.Enabled = false;
                codeGenButton.Enabled = false;
                replayButton.Enabled = false;

                InterruptTasks();
                if(NodeJsProcess != null && !NodeJsProcess.HasExited)
                    NodeJsProcess?.Kill();
                pair?.Close();
                pair = null;
                
            }
            else if (recorderState == State.Recording)
            {
                recordButton.Text = "Stop Recordings";
                optimizeButton.Enabled = false;
                codeGenButton.Enabled = false;
                replayButton.Enabled = false;
            }
        }

        private PairSocket pair = null;
        private CurrentEdit edit;

        public enum State
        {
            Connected, Disconnected, Recording
        }
        public EditUserControl()
        {
            InitializeComponent();
        }

        public void InitMain(MainForm m)
        {
            mf = m;
        }

        public void BindEdit(CurrentEdit ce)
        {
            edit = ce;
            nameTextBox.DataBindings.Clear();
            nameTextBox.DataBindings.Add("Text", edit.Thumbnail, "Name");
            ce.Recordings.ForEach(r=>LoadRecording(r));
            FilterChanged(activeFilter);
        }

        private void saveAndExitButton_Click(object sender, EventArgs e)
        {
            //TODO: save changes to name and captures recordings
            if (RecorderState == State.Recording)
            {
               DialogResult dr = MessageBox.Show(
                    "The recording is still running. Do you want to continue?",
                    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
               if (dr == DialogResult.No)
                   return;

               recordButton.PerformClick();
            }
            else if (RecorderState == State.Connected)
            {
                DialogResult dr = MessageBox.Show("The connection to browser is still active. Do you want to continue?",
                    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.No)
                    return;
                browserConnection.PerformClick();
            }

            edit.Thumbnail.Name = nameTextBox.Text;
            edit.Thumbnail.Updated = DateTime.Now;
            edit.Recordings = GetAllRecordings();
            edit.Thumbnail.Websites.UnionWith(ScrapeWebsitesFromRecordings(edit.Recordings));
            
            RecordingManager.SaveCurrentEdit(edit);

            mf.AppState = MainForm.AppMode.List;
            replayViewForm?.Close();
            actionsFlowLayoutPanel.Controls.Clear();
            filterForm?.Close();
        }

        private HashSet<string> ScrapeWebsitesFromRecordings(List<Recording> recordings)
        {
            HashSet<string> uris = new HashSet<string>();
            IEnumerable<dynamic> urlActions = recordings.Select(x=>x.Action).Where(x => x.type == "pageUrlChanged");

            string url;
            foreach (dynamic a in urlActions)
            {
                url = new Uri(a.newUrl.ToString()).Authority;
                uris.Add(url);
            }

            return uris;
        }

        private List<Recording> GetAllRecordings()
        {
            List<Recording> recordings = new List<Recording>();
            foreach (ActionUserControl auc in actionsFlowLayoutPanel.Controls)
                recordings.Add(auc.ExportRecordingForSave());

            return recordings;
        }

        private void StartNodeJsProcess()
        {
            string workingDir = Path.GetDirectoryName(Path.GetDirectoryName(edit.Config.NodeJsOptions.NodeJsEntryPoint));

            NodeJsProcess = new Process();
            NodeJsProcess.Exited += (sender, args) =>
            {
                RecorderState = State.Disconnected;
            };
            NodeJsProcess.StartInfo.WorkingDirectory = workingDir;
            NodeJsProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            NodeJsProcess.StartInfo.UseShellExecute = false;
            NodeJsProcess.StartInfo.RedirectStandardError = true;
            NodeJsProcess.StartInfo.RedirectStandardInput = true;
            NodeJsProcess.StartInfo.RedirectStandardOutput = true;

            NodeJsProcess.StartInfo.FileName = edit.Config.NodeJsOptions.InterpreterPath;
            NodeJsProcess.StartInfo.Arguments = edit.Config.NodeJsOptions.NodeJsEntryPoint;
            try
            {
            //        NodeJsProcess.Start();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool Received(string s)
        {
            string response = null;
            while (pair.HasIn)
            {
                response = pair.ReceiveFrameString();
                if (response == s)
                    return true;
            }

            bool b =pair.ReceiveFrameStringTimeout(out response, 5000);
            return b && s == response;
        }

        private void browserConnection_Click(object sender, EventArgs e)
        {
            kill = false;
            if (recorderState == State.Disconnected)
            {
                try
                {
                    StartNodeJsProcess();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Node.js app cannot be started.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if(pair == null)
                    pair = new PairSocket("@tcp://127.0.0.1:3000");


                string eventsToRecord = JsonConvert.SerializeObject(GetEventsToRecord());
                //pair.SendFrame("setEventsToRecord");
                bool b = pair.TrySendFrame(new TimeSpan(0, 0, 0, 0, 3000), "setEventsToRecord");
                if (!b)
                {
                    HandleIncorrectProcess();
                    return;
                }
                pair.SendFrame(eventsToRecord);

                if (edit.Config.PuppeteerConfig is LaunchPuppeteerOptions lpo)
                {
                    pair.SendFrame("launch");
                    pair.SendFrame(JsonConvert.SerializeObject(lpo, ConfigManager.JsonSettings));

                }
                else if (edit.Config.PuppeteerConfig is ConnectPuppeteerOptions cpo)
                {
                    pair.SendFrame("connect");
                }

                string response;
                bool received = pair.TryReceiveFrameString(new TimeSpan(0, 0, 0, 0, 8000), out response);
                if (received)
                {
                    if (response == "ACK")
                        RecorderState = State.Connected;

                    else
                    {
                        if (edit.Config.PuppeteerConfig is LaunchPuppeteerOptions)
                        {
                            MessageBox.Show("Could not launch browser, check whether chrome/chromium is installed or correct path was supplied in options", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            HandleIncorrectProcess();
                        }
                        else
                        {
                            MessageBox.Show("Could not connect to browser, check IP a Port number and whether the browser process is running with --remote-debugging-port=PORT_N", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            HandleIncorrectProcess();
                        }
                    }
                    
                }
                else
                {
                    HandleIncorrectProcess();
                }
                


            }
            else if (recorderState == State.Connected || recorderState == State.Recording)
            {
                kill = true;
                RecorderState = State.Disconnected;
                replayViewForm?.ForceClose();
            }
        }

        private void HandleIncorrectProcess()
        {
            NodeJsProcess?.Kill();
            MessageBox.Show("Node.js app is not responding. Check if node interpreter is running correct code.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void InterruptTasks()
        {
            cts?.Cancel();
            replayTask?.Wait();
            recordingTask?.Wait();
        }

        private List<string> GetEventsToRecord()
        {
            List<string> events = new List<string>() { "urlHint", "startupHints" };
            foreach (PropertyInfo pi in edit.Config.RecordedEvents.GetType().GetProperties())
            {
                if ((bool) pi.GetValue(edit.Config.RecordedEvents) && pi.CanWrite)
                    events.Add(NormalizeEventName(pi.Name));
                
            }
            return events;
        }

        private string NormalizeEventName(string eventName)
        {
            if (eventName.StartsWith("page", StringComparison.OrdinalIgnoreCase))
                return char.ToLower(eventName[0]) + eventName.Substring(1);

            return eventName.ToLower();
        }

        private bool IsBrowserConnected()
        {
            if (pair == null)
                return false;

            if (recordingTask != null && recordingTask.Status == TaskStatus.Running)
            {
                cts.Cancel();
                while (!recordingTask.IsCompleted) { }
            }

            pair.SendFrame("browserConnectionStatus");

            string bStr;
            bool res = false;
            int counter = 0;
            while ((!pair.ReceiveFrameStringTimeout(out bStr, 100) || !bool.TryParse(bStr, out res)) && counter <= 4)
            {
                pair.SendFrame("browserConnectionStatus");
                ++counter;
            }

            if (counter > 4 || !res)
            {
                MessageBox.Show("Connection to browser was lost", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                RecorderState = State.Disconnected;
                return false;
            }

            return res;
        }

        private void recordButton_Click(object sender, EventArgs e)
        {
            bool connectionStatus = IsBrowserConnected();
            if (connectionStatus && recorderState != State.Recording)
            {
                cts = new CancellationTokenSource();
                cancelToken = cts.Token;

                pair.SendFrame("start");

                RecorderState = State.Recording;
                recordingTask = Task.Factory.StartNew(RecordingTask, cancelToken);
            }
            else if(!connectionStatus)
            {
                RecorderState = State.Disconnected;
            }
            else if (connectionStatus && recorderState == State.Recording) //Stop recording
            {
                RecorderState = State.Connected;
                pair.SendFrame("stop");
            }
        }

        private void LoadAction(dynamic action)
        {
            int id;
            if (action.ToString() == "True")
                return;
            if (action.type == "urlHint")
            {
                edit.Thumbnail.Websites.Add(new Uri(action.url.ToString()).Authority);
                id = -1;
            }
            else if (action.type == "startupHints")
            {
                edit.StartupHints = action;
                id = -1;
            }
            else 
            {
                ActionUserControl auc = new ActionUserControl();
                auc.BindAction(action, edit.AllocateId());
                id = auc.Id;
                if (selectedAllCheckBox.CheckState == CheckState.Checked)
                    auc.SetSelected(true);


                UiSafeOperation(() =>
                {
                    actionsFlowLayoutPanel.Controls.Add(auc);
                    
                    if(addAsFirst.Checked)
                        actionsFlowLayoutPanel.Controls.SetChildIndex(actionsFlowLayoutPanel.Controls[actionsFlowLayoutPanel.Controls.Count - 1],0);

                    UpdateAllActionUpDownButtons();

                    actionsFlowLayoutPanel.ScrollControlIntoView(auc);
                });
                
            }

            if(addAsLast.Checked)
                edit.Recordings.Add(new Recording { Action = action, UiConfig = new UiConfig(), Id = id});
            else //addAsFirst.Checked
                edit.Recordings.Insert(0, new Recording { Action = action, UiConfig = new UiConfig(), Id = id });
        }

        private void LoadRecording(Recording r)
        {
            if (r.Action.type == "urlHint")
            {
                edit.Thumbnail.Websites.Add(new Uri(r.Action.url.ToString()).Authority);
            }
            else if (r.Action.type != "startupHints")
            {
                ActionUserControl auc = new ActionUserControl();
                auc.BindRecording(r, r.Id);
                if (selectedAllCheckBox.CheckState == CheckState.Checked)
                    auc.SetSelected(true);


                UiSafeOperation(() =>
                {
                    actionsFlowLayoutPanel.Controls.Add(auc);
                    UpdateAllActionUpDownButtons();
                });
                
            }
        }

        private void RecordingTask()
        {
            while (!cancelToken.IsCancellationRequested)
            {
                if (pair == null)
                    return;

                if (!pair.HasIn)
                    continue;

                if (pair.HasIn && cancelToken.IsCancellationRequested)
                    break;


                string json = null;
                if (!pair.TryReceiveFrameString(new TimeSpan(0, -0, 0, 0, 200), out json))
                {
                    continue;
                }

                dynamic action = JsonConvert.DeserializeObject(json, ConfigManager.JsonSettings);

                if (action.ToString() == "True")
                    continue;

                LoadAction(action);
            }
        }

        public void UpdateAllActionUpDownButtons()
        {
            foreach (ActionUserControl a in actionsFlowLayoutPanel.Controls)
                a.UpdateUpDownButtons();
        }

        private void selectedAllCheckBox_Click(object sender, EventArgs e)
        {
            bool b = true;
            if (selectedAllCheckBox.CheckState == CheckState.Indeterminate)
            {
                selectedAllCheckBox.Checked = false;
                b = false;
            }

            foreach (ActionUserControl auc in actionsFlowLayoutPanel.Controls)
            {
                auc.SetSelected(b);
            }
        }

        public void ActionUserControlCheckedChanged(ActionUserControl sender)
        {
            bool allChecked = true;
            bool allUnchecked = true;

            foreach (ActionUserControl auc in actionsFlowLayoutPanel.Controls)
            {
                if (!auc.Selected)
                {
                    allChecked = false;
                }
                else if (auc.Selected)
                {
                    allUnchecked = false;
                }
                if (!allChecked && !allUnchecked)
                {
                    selectedAllCheckBox.CheckState = CheckState.Indeterminate;
                    break;
                }
            }

            if (allChecked)
                selectedAllCheckBox.CheckState = CheckState.Checked;
            
            
            
            else if (allUnchecked)
                selectedAllCheckBox.CheckState = CheckState.Unchecked;

            UpdateProcessButtons();
        }

        private void UpdateProcessButtons()
        {
            if (RecorderState == State.Connected)
            {
                optimizeButton.Enabled = SomeActionsSelectedForProcessing();
                codeGenButton.Enabled = SomeActionsSelectedForProcessing();
                replayButton.Enabled = SomeActionsSelectedForProcessing();
            }
        }

        public void ActionUserControlEnableCheckedChanged(ActionUserControl sender)
        {
            UpdateProcessButtons();
        }

        private void filterButton_Click(object sender, EventArgs e)
        {
            filterForm = new FilterForm(this);
            if (activeFilter != null)
                filterForm.SetFilter(activeFilter);

            filterForm.Show();
        }

        private Tuple<List<dynamic>, List<ActionUserControl>> GetRecordingActionsForOutput()
        {
            Tuple<List<dynamic>, List<ActionUserControl>> ret;
            List<dynamic> outputActions = new List<dynamic>();
            List<ActionUserControl> outputUserControl = new List<ActionUserControl>();

            if (edit.StartupHints != null)
                outputActions.Add(edit.StartupHints);
            
            foreach (ActionUserControl auc in actionsFlowLayoutPanel.Controls)
            {
                if (enabledRadioButton.Checked)
                {
                    if (auc.EnabledForOutput)
                    {
                        outputActions.Add(auc.ExportActionForOutput());
                        outputUserControl.Add(auc);
                    }
                }
                else if (selectedRadioButton.Checked)
                {
                    if (auc.Selected)
                    {
                        outputActions.Add(auc.ExportActionForOutput());
                        outputUserControl.Add(auc);
                    }
                }
                
            }

            ret = new Tuple<List<dynamic>, List<ActionUserControl>>(outputActions, outputUserControl);
            return ret;
        }

        private List<Recording> GetRecordingsForOptimize()
        {
            List<Recording> outputActions = new List<Recording>();
            foreach (ActionUserControl auc in actionsFlowLayoutPanel.Controls)
            {
                if (enabledRadioButton.Checked)
                {
                    if (auc.EnabledForOutput)
                        outputActions.Add(auc.ExportRecordingForSave());
                }
                else if (selectedRadioButton.Checked)
                {
                    if (auc.Selected)
                        outputActions.Add(auc.ExportRecordingForSave());
                }

            }

      //      outputActions[0].
            return outputActions;
        }

        private void optimizeButton_Click(object sender, EventArgs e)
        {
            if (actionsFlowLayoutPanel.Controls.Count == 0)
                return;

            string json = JsonConvert.SerializeObject(GetRecordingsForOptimize(), ConfigManager.JsonSettings);
            pair.SendFrame("optimize");
            pair.SendFrame(json);
            json = pair.ReceiveFrameString();
            List<Recording> actions = JsonConvert.DeserializeObject<List<Recording>>(json, ConfigManager.JsonSettings);
            edit.Recordings = actions;

            actionsFlowLayoutPanel.Controls.Clear();
            actions.ForEach(a => LoadRecording(a));
        }

        private void processRadioButtons_CheckedChanged(object sender, EventArgs e)
        {
            UpdateProcessButtons();
        }

        private void codeGenButton_Click(object sender, EventArgs e)
        {
            string actionsJson = JsonConvert.SerializeObject(GetRecordingActionsForOutput().Item1, ConfigManager.JsonSettings);
            string codeGenOptsJson = JsonConvert.SerializeObject(edit.Config.CodeGeneratorConfig, ConfigManager.JsonSettings);
            pair.SendFrame("codeGen");
            pair.SendFrame(codeGenOptsJson);
            pair.SendFrame(actionsJson);
            string code = pair.ReceiveFrameString();
            CodeGenEditor cge = new CodeGenEditor();
            cge.SetEditorText(code);
            cge.Show();
        }

        private void replayButton_Click(object sender, EventArgs e)
        {
            if (IsBrowserConnected())
            {
                cts = new CancellationTokenSource();
                cancelToken = cts.Token;
                replayViewForm?.Close();
                replayViewForm = new ReplayViewForm(this, cts);
                SetReplayEndedVisibility(false);
                replayViewForm.Show();

                replayTask = Task.Factory.StartNew(ReplayTask, cancelToken);
            }

        }

        public void HighlightActionUserControlById(int id, Color c)
        {
            lock (colorChangeLck)
            {
                if (oldErrorHighlightedAuc != null)
                    UiSafeOperation(() =>
                    {
                        oldErrorHighlightedAuc.BackColor = default;
                        actionsFlowLayoutPanel.ScrollControlIntoView(runningNowHighlightedAuc);
                    });

                if (runningNowHighlightedAuc != null)
                {
                    UiSafeOperation(() =>
                    {
                        runningNowHighlightedAuc.BackColor = Color.DarkGoldenrod;
                        actionsFlowLayoutPanel.ScrollControlIntoView(runningNowHighlightedAuc);
                    });
                }

                foreach (ActionUserControl auc in actionsFlowLayoutPanel.Controls)
                {
                    if (auc.Id == id)
                    {
                        oldErrorHighlightedAuc = auc;
                        auc.BackColor = c;
                        actionsFlowLayoutPanel.ScrollControlIntoView(auc);
                        break;
                    }
                }
            }
        }

        public void ClearErrorCustomColors()
        {
            lock (colorChangeLck)
            {
                if (oldErrorHighlightedAuc != null)
                {
                    UiSafeOperation(() => oldErrorHighlightedAuc.BackColor = default);
                    oldErrorHighlightedAuc = null;
                    if (runningNowHighlightedAuc != null)
                    {
                        UiSafeOperation(() =>
                        {
                            runningNowHighlightedAuc.BackColor = Color.DarkGoldenrod;
                            actionsFlowLayoutPanel.ScrollControlIntoView(runningNowHighlightedAuc);
                        });
                    }
                }
            }
        }

        private void SetReplayActiveUi(bool state)
        {
            UiSafeOperation(() =>
            {
                recordButton.Enabled = !state;
                optimizeButton.Enabled = !state;
                replayButton.Enabled = !state;
                codeGenButton.Enabled = !state;


                foreach (ActionUserControl auc in actionsFlowLayoutPanel.Controls)
                {
                    auc.Enabled = !state;
                }
            });
        }

        private void AddErrorToReplayViewForm(string msg, int id)
        {
            UiSafeOperation(() => replayViewForm.AddError(msg, id));
        }

        private void SetReplayEndedVisibility(bool visibility)
        {
            UiSafeOperation(() => replayViewForm.SetRecordingEnded(visibility));
        }

        private void UiSafeOperation(Action a)
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(() => a()));

            else
                a();
        }

        public void ClearAucCustomColors()
        {
            lock (colorChangeLck)
            {
                foreach (ActionUserControl auc in actionsFlowLayoutPanel.Controls)
                {
                    UiSafeOperation(() => auc.BackColor = default);
                }
            }
        }

        private void FinishReplay()
        {
            pair.SendFrame("finished");
            string msg = pair.ReceiveFrameString();
            while (msg != "evaluated")
            {
                msg = pair.ReceiveFrameString();
            }
            runningNowHighlightedAuc = null;
            UiSafeOperation(() => replayViewForm.ReplayEndedNow());
            SetReplayActiveUi(false);

        }

        public void DeleteRequested()
        {
            List<ActionUserControl> toRemove = new List<ActionUserControl>();
            foreach (ActionUserControl auc in actionsFlowLayoutPanel.Controls)
            {
                if(auc.Selected) 
                    toRemove.Add(auc);
            }
            toRemove.ForEach(auc => actionsFlowLayoutPanel.Controls.Remove(auc));
            selectedAllCheckBox.CheckState = CheckState.Unchecked;
        }

        private void AucColorChangeSafe(ActionUserControl auc, Color c)
        {
            lock (colorChangeLck)
            {
                UiSafeOperation(() =>
                {
                    auc.BackColor = c;
                    actionsFlowLayoutPanel.ScrollControlIntoView(auc);
                });
                runningNowHighlightedAuc = auc;
            }
        }

        private void ReplayTask()
        {
            SetReplayActiveUi(true);

            Tuple<List<dynamic>, List<ActionUserControl>> t = GetRecordingActionsForOutput();
            List<dynamic> actions = t.Item1;
            List<ActionUserControl> aucs = t.Item2;

            string codeGenConfig = JsonConvert.SerializeObject(edit.Config.PlayerOptions, ConfigManager.JsonSettings);
            string actionsJson = JsonConvert.SerializeObject(actions, ConfigManager.JsonSettings);
            pair.SendFrame("replay");
            pair.SendFrame(codeGenConfig);
            pair.SendFrame(actionsJson);

            actions.RemoveAt(0); //startupHints

            for (int i = 0; i < actions.Count; ++i)
            {

                if (cts.IsCancellationRequested)
                {
                    if(!kill)
                        FinishReplay();
                    return;
                }

                lock (colorChangeLck)
                {
                    runningNowHighlightedAuc = aucs[i];
                }
                AucColorChangeSafe(aucs[i], Color.DarkGoldenrod);

                pair.SendFrame(i.ToString());
                bool b;
                string m = null;
                b = pair.ReceiveFrameStringTimeout(out m, 100);
                while (!b && !cts.IsCancellationRequested)
                    b = pair.ReceiveFrameStringTimeout(out m, 100);


                if (cts.IsCancellationRequested)
                {
                    if (!kill)
                    {
                        FinishReplay();
                        AucColorChangeSafe(aucs[i], default);
                    }

                    return;
                }

                while (m != "evaluated")
                {
                    //exception occurred
                    if(m != "true")
                        AddErrorToReplayViewForm(m, aucs[i].Id);

                    b = false;
                    while (!b && !cts.IsCancellationRequested)
                        b = pair.ReceiveFrameStringTimeout(out m, 100);


                    if (cts.IsCancellationRequested)
                    {
                        if (!kill)
                        {
                            FinishReplay();
                            AucColorChangeSafe(aucs[i], default);
                        }
                        return;
                    }


                }

                AucColorChangeSafe(aucs[i], default);
            }

            runningNowHighlightedAuc = null;
            pair.SendFrame("finished");

            SetReplayEndedVisibility(true);
            SetReplayActiveUi(false);

            //SetReplayActiveUi(true);
            //string actionsJson = JsonConvert.SerializeObject(GetRecordingActionsForOutput(), ConfigManager.JsonSettings);
            //string codeGenConfig = JsonConvert.SerializeObject(edit.Config.PlayerOptions, ConfigManager.JsonSettings);
            //pair.SendFrame("replay");
            //pair.SendFrame(codeGenConfig);
            //pair.SendFrame(actionsJson);

            //string msg = null;
            //bool b = false;

            //while (!cts.IsCancellationRequested && msg != "evaluated")
            //{
            //    if (b)
            //    {
            //        AddErrorToReplayViewForm(msg);
            //        //Add error to error list
            //    }
            //    b = pair.ReceiveFrameStringTimeout(out msg, 100);
            //}
            //if (msg == "evaluated")
            //{
            //    SetReplayActiveUi(false);
            //    SetReplayEndedVisibility(true);
            //}
            //if (cts.IsCancellationRequested)
            //{
            //    UiSafeOperation(() => replayViewForm?.Close());
            //    NodeJsProcess.Kill();
            //    SetReplayActiveUi(false);
            //}
        }
    }
}
