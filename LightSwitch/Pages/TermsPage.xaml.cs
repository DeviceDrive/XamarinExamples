using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Autofac;
using Xamarin.Forms;
using DeviceDrive.SDK;
using DeviceDrive.SDK.Contracts;

namespace LightSwitch
{
	public partial class TermsPage : ContentPage
	{
        ITermsService _termsService;
        String termsConditionsContent = "terms";
        String privacyPolicyContent = "policy";

        Command _showTermsConditionsCommand;
        Command _showPrivacyPolicyCommand;
        Command _acceptCommand;
        Command _rejectCommand;

        public TermsPage(IContainer container)
		{
			Title = "Terms";
			InitializeComponent();
			BindingContext = this;

            //TermsConditionsButtonColor = Color.Black;
            //PrivacyPolicyButtonColor = ColorConstants.LightBlueTextColor;

            using (var scope = container.BeginLifetimeScope())
            {
                _termsService = scope.Resolve<ITermsService>();
            }
            GetTermsAndPolicy();
        }

        async void GetTermsAndPolicy()
        {
            termsConditionsContent = await _termsService.GetDecodedTermsConditions();
            privacyPolicyContent = await _termsService.GetDecodedPrivacyPolicy();
            var htmlSource = new HtmlWebViewSource();
            htmlSource.Html = termsConditionsContent;
            CurrentContent = htmlSource;
        }

        #region Properties

        /// <summary>
        /// The StatusText property.
        /// </summary>
        public static BindableProperty TermsConditionsButtonColorProperty = BindableProperty.Create(
            nameof(TermsConditionsButtonColor), typeof(string), typeof(TermsPage), null,
            BindingMode.OneWay);

        /// <summary>
        /// Gets or sets the StatusText of the MainPage instance.
        /// </summary>
        public string TermsConditionsButtonColor
        {
            get { return (string)GetValue(TermsConditionsButtonColorProperty); }
            set { SetValue(TermsConditionsButtonColorProperty, value); }
        }


        /// <summary>
        /// The StatusText property.
        /// </summary>
        public static BindableProperty PrivacyPolicyButtonColorProperty = BindableProperty.Create(
            nameof(PrivacyPolicyButtonColor), typeof(string), typeof(TermsPage), null,
            BindingMode.OneWay);

        /// <summary>
        /// Gets or sets the StatusText of the MainPage instance.
        /// </summary>
        public string PrivacyPolicyButtonColor
        {
            get { return (string)GetValue(PrivacyPolicyButtonColorProperty); }
            set { SetValue(PrivacyPolicyButtonColorProperty, value); }
        }


        /// <summary>
        /// The StatusText property.
        /// </summary>
        public static BindableProperty HtmlWebViewSourceProperty = BindableProperty.Create(
            nameof(CurrentContent), typeof(HtmlWebViewSource), typeof(TermsPage), null,
            BindingMode.OneWay);

        /// <summary>
        /// Gets or sets the StatusText of the MainPage instance.
        /// </summary>
        public HtmlWebViewSource CurrentContent
        {
            get { return (HtmlWebViewSource)GetValue(HtmlWebViewSourceProperty); }
            set { SetValue(HtmlWebViewSourceProperty, value); }
        }

        /// <summary>
		/// Returns the command for showing terms/conditions content
		/// </summary>
		public Command ShowTermsConditionsCommand
        {
            get
            {
                return _showTermsConditionsCommand ?? (_showTermsConditionsCommand = new Command(async () =>
                {
                    //TermsConditionsButtonColor = Color.Black;
                    //PrivacyPolicyButtonColor = ColorConstants.LightBlueTextColor;
                    var htmlSource = new HtmlWebViewSource();
                    htmlSource.Html = termsConditionsContent;
                    CurrentContent = htmlSource;
                }));
            }
        }

        /// <summary>
		/// Returns the command for showing privacy policy content
		/// </summary>
		public Command ShowPrivacyPolicyCommand
        {
            get
            {
                return _showPrivacyPolicyCommand ?? (_showPrivacyPolicyCommand = new Command(async () =>
                {
                    //TermsConditionsButtonColor = ColorConstants.LightBlueTextColor;
                    //PrivacyPolicyButtonColor = Color.Black;
                    var htmlSource = new HtmlWebViewSource();
                    htmlSource.Html = privacyPolicyContent;
                    CurrentContent = htmlSource;
                }));
            }
        }

        /// <summary>
		/// Returns the command for accepting terms/conditions and privacy policy
		/// </summary>
		public Command AcceptCommand
        {
            get
            {
                return _acceptCommand ?? (_acceptCommand = new Command(async () =>
                {
                    await Navigation.PopModalAsync(true);
                    await _termsService.Accept();
                }));
            }
        }

        /// <summary>
		/// Returns the command for rejecting terms/conditions and privacy policy
		/// </summary>
		public Command RejectCommand
        {
            get
            {
                return _rejectCommand ?? (_rejectCommand = new Command(async () =>
                {
                    var res = await DisplayAlert("LightSwitch", "Are you sure to reject terms and privacy policy? Please notice that terms and privacy policy have to be accepted in order to use the app. Rejection will take you to the initial screen.", "Yes", "No");
                    if (res)
                    {
                        await _termsService.Reject();
                        await Navigation.PopModalAsync(true);
                        await DeviceDriveManager.Current.Authentication.LogoutAsync();
                    }
                }));
            }
        }

        #endregion

        #region Commands

        #endregion

        #region Private Members

        #endregion
    }
}
