using System;

namespace TesseractOCR.Enums
{
    internal class StringAttribute : Attribute
    {
        #region Properties
        public string Value { get; }
        #endregion

        #region Constructor
        internal StringAttribute(string value)
        {
            Value = value;
        }
        #endregion
    }
}
