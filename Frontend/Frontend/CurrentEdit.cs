﻿using System.Collections.Generic;

namespace Frontend
{
    /// <summary>
    /// This class represents the actual recording with UI related stuff (Thumbnail), configuration (Config), etc.
    /// </summary>
    public class CurrentEdit
    {
        public Thumbnail Thumbnail { get; set; }
        public dynamic StartupHints { get; set; }
        public List<Recording> Recordings { get; set; }
        public Configuration Config { get; set; }
        public int NextAvailableId { get; set; }

        public int AllocateId()
        {
            return NextAvailableId++;
        }
    }
}
