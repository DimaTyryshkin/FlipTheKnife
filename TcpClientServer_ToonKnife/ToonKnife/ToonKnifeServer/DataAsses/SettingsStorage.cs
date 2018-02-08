using Assets.game.model.knife;
using CodeWriter.Config;
using OEPFramework;
using System;

namespace ToonKnife.Server.DataAsses
{
    public class SettingsStorage
    {
        RawNode _knifeSettings;
        RawNode _settings;

        //---prop
        public RawNode KnifeSettings => _knifeSettings;

        public RawNode Settings => _settings;


        //---methods
        public SettingsStorage(IConfig config)
        {
            _knifeSettings = config.GetRawNode("knives.json");
            _settings = config.GetRawNode("settings.json");
        }
    }
}