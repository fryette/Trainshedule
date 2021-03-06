using System.Collections.Generic;
using Cirrious.MvvmCross.ViewModels;
using Newtonsoft.Json;
using Trains.Core.Interfaces;
using Trains.Core.Resources;
using Trains.Model.Entities;

namespace Trains.Core.ViewModels
{
	public class HelpViewModel : MvxViewModel
	{
		#region readonlyProperties

		private readonly IAppSettings _appSettings;

		#endregion

		#region commands

		public MvxCommand<CarriageModel> SelectCarriageCommand { get; private set; }

		#endregion

		#region ctor

		public HelpViewModel(IAppSettings appSettings)
		{
			_appSettings = appSettings;
			SelectCarriageCommand = new MvxCommand<CarriageModel>(SelectCarriage);
		}

		#endregion

		#region properties

		#region UIproperties

		public string Carriage { get; set; }
		public string Trains { get; set; }
		public string Other { get; set; }

		#endregion
		/// <summary>
		/// Used to dispalying informations about belarussian railway icons.
		/// </summary>
		private IEnumerable<HelpInformationItem> _helpInformation;
		public IEnumerable<HelpInformationItem> HelpInformation
		{
			get { return _helpInformation; }
			set
			{
				_helpInformation = value;
				RaisePropertyChanged(() => HelpInformation);
			}
		}

		private List<CarriageModel> _carriageInformation;
		public List<CarriageModel> CarriageInformation
		{
			get { return _carriageInformation; }
			set
			{
				_carriageInformation = value;
				RaisePropertyChanged(() => CarriageInformation);
			}
		}

		public List<PlaceInformation> PlaceInformation { get; set; }
		//public List<string> PlaceInformation
		//{
		//    get { return new List<string>{} }
		//}

		#endregion

		#region action

		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		public void Init()
		{
			RestoreUiBindings();
			HelpInformation = _appSettings.HelpInformation;
			CarriageInformation = _appSettings.CarriageModel;
			PlaceInformation = _appSettings.PlaceInformation;
		}

		private void SelectCarriage(CarriageModel selectedCarriageModel)
		{
			if (selectedCarriageModel == null) return;
			ShowViewModel<CarriageViewModel>(new { param = JsonConvert.SerializeObject(selectedCarriageModel) });
		}

		private void RestoreUiBindings()
		{
			Other = ResourceLoader.Instance.Resource["Other"];
			Carriage = ResourceLoader.Instance.Resource["Carriage"];
			Trains = ResourceLoader.Instance.Resource["Trains"];
		}

		#endregion
	}
}