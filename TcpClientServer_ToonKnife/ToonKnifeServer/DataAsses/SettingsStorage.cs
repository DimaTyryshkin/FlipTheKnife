using Assets.game.model.knife;
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

        public RawNode Settings => _knifeSettings;


        //---methods
        public SettingsStorage()
        {
            throw new NotImplementedException();
        }
    }
}