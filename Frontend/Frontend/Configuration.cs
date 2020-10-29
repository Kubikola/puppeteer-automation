﻿namespace Frontend
{
    public class Configuration
    {
        public PuppeteerOptions PuppeteerConfig { get; set; }
        public CodeGeneratorOptions CodeGeneratorConfig { get; set; }
        public PlayerOptions PlayerOptions { get; set; }
        public NodeJsOptions NodeJsOptions { get; set; }
        public RecordedEvents RecordedEvents { get; set; }
    }
}
