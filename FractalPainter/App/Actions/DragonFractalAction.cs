using System;
using FractalPainting.App.Fractals;
using FractalPainting.Infrastructure.Common;
using FractalPainting.Infrastructure.Injection;
using FractalPainting.Infrastructure.UiActions;
using Ninject;

namespace FractalPainting.App.Actions
{
	public class DragonFractalAction : IUiAction
	{
	    private readonly IFactory dragonFactory;
	    private readonly Func<Random, DragonSettingsGenerator> settingsGeneratorFactory;

	    public DragonFractalAction(
            IFactory dragonFactory, 
            Func<Random, DragonSettingsGenerator> settingsGeneratorFactory)
	    {
	        this.dragonFactory = dragonFactory;
	        this.settingsGeneratorFactory = settingsGeneratorFactory;
	    }

	    public string Category => "Фракталы";
		public string Name => "Дракон";
		public string Description => "Дракон Хартера-Хейтуэя";

		public void Perform()
		{
			var dragonSettings = settingsGeneratorFactory(new Random()).Generate();
			// редактируем настройки:
			SettingsForm.For(dragonSettings).ShowDialog();
			// создаём painter с такими настройками
		    var dragonPainter = dragonFactory.CreaDragonPainter(dragonSettings);
            dragonPainter.Paint();
		}
	}
}