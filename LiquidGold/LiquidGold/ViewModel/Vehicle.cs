using System;
using System.Net;
using System.Windows;

namespace LiquidGold.ViewModel
{
    public class Vehicle
    {
        /// <summary>
        /// 
        /// </summary>
        private string _name;

        /// <summary>
        /// 
        /// </summary>
        private string _image;

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Image
        {
            get { return _image; }
            set
            {
                if (value != _image)
                {
                    _image = value;
                }
            }
        }
    }
}
