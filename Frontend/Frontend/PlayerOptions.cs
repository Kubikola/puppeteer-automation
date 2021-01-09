﻿using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Frontend
{
    /// <summary>
    /// Options that change the behaviour of a playback.
    /// </summary>
    public class PlayerOptions
    {
        [Browsable(false)]
        [JsonProperty(PropertyName = "browserDisconnect")]
        private bool browserDisconnect = false;

        [Browsable(false)]
        [JsonProperty(PropertyName = "browserClose")]
        private bool browserClose = false;

        [JsonProperty(PropertyName = "waitForNavigationOptions")]
        [Browsable(false)]
        public string WaitForNavigationOptionJson => "{ \"waitUntil\": \"" + Enum.GetName(typeof(WaitForNavigation), WaitForNavigationOptions) + "\" }";


        [Description("Defines a condition until the script should wait after performing a navigation.")]
        public WaitForNavigation WaitForNavigationOptions { get; set; }

        [JsonProperty(PropertyName = "waitForTargetOptions")]
        [Description("Time that should be waited for elements in waitForSelector and waitForXPath statements.")]
        [Browsable(false)]
        public string WaitForTargetOptions => "{ \"timeout\": " + WaitForTargetTimeoutMs + " }";

        [JsonIgnore]
        public int WaitForTargetTimeoutMs { get; set; } = 5000;

        [Browsable(false)]
        [JsonProperty(PropertyName = "catchErrors")]
        public bool CatchErrors { get; set; } = true;

        [Browsable(false)]
        [JsonProperty(PropertyName = "logErrors")]
        public bool LogErrors { get; set; } = true;

        [JsonProperty(PropertyName = "sendErrorsBack")]
        public bool SendErrorsBack = true;

        [JsonProperty(PropertyName = "typeOptions")]
        [Browsable(false)]
        public string TypeOptions => "{ \"delay\": " + KeystrokeDelayMs + " }";

        [JsonIgnore]
        [Description("Sets a pause (ms) between each keystroke.")]
        public int KeystrokeDelayMs { get; set; } = 100;

        [Browsable(false)]
        [JsonProperty(PropertyName = "evaluationFinishedAck")]
        public bool EvaluationFinishedAck { get; set; } = false;
    }
}
