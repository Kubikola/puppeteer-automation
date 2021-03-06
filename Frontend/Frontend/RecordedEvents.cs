﻿using System.ComponentModel;

namespace Frontend
{
    /// <summary>
    /// Contains events that are available for capturing.
    /// </summary>
    public class RecordedEvents
    {
        public bool Click { get; set; } = true;
        public bool DblClick { get; set; } = true;
        public bool Change { get; set; } = true;
        public bool Input { get; set; } = true;
        public bool Select { get; set; } = true;
        public bool Submit { get; set; } = true;
        public bool Scroll { get; set; } = true;
        public bool Copy { get; set; } = true;
        public bool Paste { get; set; } = true;

        public bool PageClosed { get; set; } = true;
        public bool PageSwitched { get; set; } = true;
        public bool PageOpened { get; set; } = true;
        public bool PageUrlChanged { get; set; } = true;

        [Description("Hold 'h' key to record.")]
        public bool MouseOver { get; } = true;
    }
}
