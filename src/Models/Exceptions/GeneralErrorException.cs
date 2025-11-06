using System;

namespace eTeacherProject.Models.Exceptions
{
    public class GeneralErrorException : Exception
    {
        public GeneralErrorException(string message)
            : base(message) { }
    }
}
