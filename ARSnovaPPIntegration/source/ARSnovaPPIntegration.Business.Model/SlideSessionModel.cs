﻿using System.Collections.ObjectModel;

using ARSnovaPPIntegration.Common.Enum;

namespace ARSnovaPPIntegration.Business.Model
{
    public class SlideSessionModel
    {
        public ObservableCollection<SlideQuestionModel> Questions { get; set; } = new ObservableCollection<SlideQuestionModel>();

        // in case of arsnova.voting, the session id is saved as the hashtag
        public string Hashtag { get; set; }

        public string ArsnovaVotingSessionName { get; set; }

        public string ArsnovaVotingSessionShortName { get; set; }

        public string PrivateKey { get; set; }

        public int IntroSlideId { get; set; }

        public SessionType SessionType { get; set; } = SessionType.ArsnovaClick;

        public bool SessionTypeSet { get; set; } = false;

        public ArsnovaEuConfig ArsnovaEuConfig { get; set; }
    }
}
