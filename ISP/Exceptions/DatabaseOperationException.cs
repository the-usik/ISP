﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISP.Exceptions {
    public class DatabaseOperationException : Exception {
        public DatabaseOperationException(string message) : base(message) { }
    }
}
