﻿using System.Collections.Generic;
using System.Windows.Input;

using Microsoft.Practices.ServiceLocation;

using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Common.Enum;
using ARSnovaPPIntegration.Presentation.Commands;
using ARSnovaPPIntegration.Presentation.Window;

namespace ARSnovaPPIntegration.Presentation.Models
{
    public class QuestionViewModel : BaseModel
    {
        public QuestionViewModel(
            ViewPresenter.ViewPresenter viewPresenter,
            ILocalizationService localizationService,
            SlideSessionModel slideSessionModel) 
            : base(viewPresenter, localizationService, slideSessionModel)
        {
            this.InitializeWindowCommandBindings();

            var sessionInformationProvider = ServiceLocator.Current.GetInstance<ISessionInformationProvider>();

            this.QuestionTypes = slideSessionModel.SessionType == SessionType.ArsnovaClick
                ? sessionInformationProvider.GetAvailableQuestionsClick()
                : sessionInformationProvider.GetAvailableQuestionsVoting();
        }

        public string Header => this.LocalizationService.Translate("Set question");

        public string Text => this.LocalizationService.Translate("Choose a question type and enter the question text:");

        public List<QuestionType> QuestionTypes { get; set; }

        public QuestionTypeEnum QuestionType
        {
            get
            {
                if (this.SlideSessionModel.QuestionType != 0)
                {
                    return this.SlideSessionModel.QuestionType;
                }
                else
                {
                    return this.SlideSessionModel.SessionType == SessionType.ArsnovaClick
                     ? QuestionTypeEnum.SingleChoiceClick
                     : QuestionTypeEnum.SingleChoiceVoting;
                }
            }
            set
            {
                if (this.SlideSessionModel.QuestionTypeSet || this.SlideSessionModel.AnswerOptionsSet)
                {
                    var reset = PopUpWindow.ConfirmationWindow(
                        this.LocalizationService.Translate("Reset"),
                        this.LocalizationService.Translate(
                                "If this value is changed, other Session-Properties like the answer options or the question type will be reseted. Do you want to continue?"));

                    if (reset)
                    {
                        this.SlideSessionModel.QuestionType = value;
                        this.SlideSessionModel.AnswerOptions = null;
                        this.SlideSessionModel.AnswerOptionsSet = false;
                    }
                }
                else
                {
                    this.SlideSessionModel.QuestionType = value;
                    this.SlideSessionModel.QuestionTypeSet = true;
                }
            }
        }

        public string QuestionText
        {
            get { return this.SlideSessionModel.QuestionText; }
            set { this.SlideSessionModel.QuestionText = value; }
        }

        private void InitializeWindowCommandBindings()
        {
            this.WindowCommandBindings.AddRange(
                    new List<CommandBinding>
                    {
                        new CommandBinding(
                            NavigationButtonCommands.Back,
                            (e, o) =>
                            {
                                this.ViewPresenter.Show(
                                    new SelectArsnovaTypeViewModel(
                                        this.ViewPresenter,
                                        this.LocalizationService,
                                        this.SlideSessionModel));
                            },
                            (e, o) => o.CanExecute = true),
                        new CommandBinding(
                            NavigationButtonCommands.Forward,
                            (e, o) =>
                            {
                                this.ViewPresenter.Show(
                                    new AnswerOptionViewModel(
                                        this.ViewPresenter,
                                        this.LocalizationService,
                                        this.SlideSessionModel));
                            },
                            (e, o) => o.CanExecute = true)
                    });
        }
    }
}
