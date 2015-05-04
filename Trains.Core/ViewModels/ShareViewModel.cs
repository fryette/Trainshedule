﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cirrious.MvvmCross.ViewModels;
using Trains.Model.Entities;

namespace Trains.Core.ViewModels
{
    public class ShareViewModel : MvxViewModel
    {

        #region UIproperties

        public string ShareHeader { get; set; }
        public string ShareTitle { get; set; }

        #endregion
        public IEnumerable<ShareSocial> ShareItems { get; set; }
        public void Init()
        {
            ShareItems = new List<ShareSocial>
            {
                ShareSocial.Vkontakte,
                ShareSocial.Facebook,
                ShareSocial.Twitter,
                ShareSocial.GooglePlus,
                ShareSocial.LinkedIn,
                ShareSocial.Odnoklassniki
            };
        }
    }
}
