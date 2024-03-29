﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Army.Domain.Dto
{
    public class VideoSourceDto
    {

        public string Content { get; set; }
        public string Value { get; set; }
        public string Url { get; set; }

        public bool Enabled { get; set; } = true;
    }
}
