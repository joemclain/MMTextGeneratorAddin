using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using FontAwesome.WPF;
using MarkdownMonster;
using MarkdownMonster.AddIns;

using LoremIpsum;

using TextGenerator.Forms;

namespace TextGenerator {


	/////////////////////////////////////////////////////////////////////////////

	public class AddinModel {

		public AppModel AppModel { get; }
		public TextGeneratorAddin Addin { get; }
		public AddinConfiguration Configuration { get; }
		public TextGenForm Form { get; set; }
		public ITextGenerator LoremIpsum { get; } = new LoremIpsum.TextGenerator { };


		/////////////////////////////////////////////////////////////////////////////

		public AddinModel( AppModel appModel, TextGeneratorAddin addin )
		{
			AppModel = appModel;
			Addin = addin;
			Configuration = AddinConfiguration.Current;
		}

	}



	/////////////////////////////////////////////////////////////////////////////

	public class TextGeneratorAddin : MarkdownMonsterAddin {

		private AddinModel AddinModel { get; set; }


		/////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Determines on whether the addin can be executed
		/// </summary>
		/// <param name="sender">The Execute toolbar button for this addin</param>
		/// <returns></returns>

		public override bool OnCanExecute( object sender )
		{
			return true;
		}

		/////////////////////////////////////////////////////////////////////////////

		public override async Task OnApplicationStart()
		{
			// ******
			await base.OnApplicationStart();

			// ******
			Id = "TextGenerator";
			Name = "TextGenerator";

			// ******
			//
			// menu item on tools->Add-ins menu
			//
			var menuItem = new AddInMenuItem( this ) {
				Caption = Name,
				//
				// icon on toolbar
				//
				FontawesomeIcon = FontAwesomeIcon.Paw,

#if DEBUG
				//
				// for quick access during debug
				//
				KeyboardShortcut = AddinConfiguration.Current.KeyboardShortcut
#endif
			};


			// ******
			//
			// add to menu items to display at tools->Add-ins
			//
			MenuItems.Add( menuItem );

			// ******
			await Task.CompletedTask;
		}


		/////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Fired after the model has been loaded. If you need model access during loading
		/// this is the place to hook up your code.
		/// </summary>
		/// <param name="model">The Markdown Monster Application model</param>

		public override Task OnModelLoaded( AppModel model )
		{
			// ******
			this.Model = model;
			AddinModel = new AddinModel( model, this );

			// ******
			return Task.CompletedTask;
		}


		/////////////////////////////////////////////////////////////////////////////

		//private MenuItem MainMenuItem;
		//
		//void AddMainMenuItems()
		//{
		//	// create commands
		//	//Command_WeblogForm();
		//	//Command_WebLogSearch();
		//
		//
		//	MainMenuItem = new MenuItem {
		//		Header = "Text Generator"
		//	};
		//
		//	//AddMenuItem( MainMenuItem, "MainMenuTools", addMode: AddMenuItemModes.AddBefore );
		//	AddMenuItem( menuItem, "MainMenuTools", addMode: AddMenuItemModes.AddBefore );
		//	
		//	MainMenuItem.Items.Add( new Separator() );
		//
		//	//MainMenuItem.SubmenuOpened += MainMenuItem_SubmenuOpened;
		//}


		/////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Fired after the Markdown Monster UI becomes available
		/// for manipulation.
		///
		/// If you add UI elements as part of your Addin, this is the
		/// place where you can hook them up.
		/// </summary>

		public override Task OnWindowLoaded()
		{
			//AddMainMenuItems();
			return Task.CompletedTask;
		}


		/////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Fired when you click the addin button in the toolbar.
		/// </summary>
		/// <param name="sender"></param>

		public override async Task OnExecute( object sender )
		{
			// ******
			if( null != AddinModel.Form ) {
				AddinModel.Form.Topmost = true;
				AddinModel.Form.Focus();
			}
			else {
				var tgForm = AddinModel.Form = new TextGenForm(  AddinModel, () => AddinModel.Form = null );
				tgForm.Show();
			}

			// ******
			await Task.CompletedTask;
		}


		/////////////////////////////////////////////////////////////////////////////
		// /// <summary>
		// /// Fired when you click on the configuration button in the addin
		// /// </summary>
		// /// <param name="sender">The Execute toolbar button for this addin</param>
		// 
		// public override Task OnExecuteConfiguration( object sender )
		// {
		// 	base.OnExecuteConfiguration( sender );
		// 
		// 	MessageBox.Show( "Configuration for our sample Addin",
		// 		"Markdown Addin Sample",
		// 		MessageBoxButton.OK, MessageBoxImage.Information );
		// 
		// 	return Task.CompletedTask;
		// }


	}
}
