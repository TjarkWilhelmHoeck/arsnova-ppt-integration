﻿using ARSnovaPPIntegration.Common.Contract;
using Microsoft.Practices.ServiceLocation;

namespace ARSnovaPPIntegration.Presentation.Window
{
    public class NavigationButtonsToolTips
    {
        private readonly ILocalizationService localizationService;

        public NavigationButtonsToolTips ()
        {
            this.localizationService = ServiceLocator.Current.GetInstance<ILocalizationService>();
        }

        public string Back => this.localizationService.Translate("Back");
    }
}