using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISP.Exceptions {

    class AuthorizationException : Exception {
        public enum AuthErrorType {
            IncorrectLoginOrPassword,
            IncorrectData,
            LoginAlreadyExists,
            Unknown
        }

        public AuthErrorType ErrorType;

        public AuthorizationException(AuthErrorType type, string message) : base(message) {
            ErrorType = type;
        }
    }
}
