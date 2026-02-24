// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using System.ComponentModel;
namespace osu.Game.Configuration
{
    public enum ReleaseStream
    {
        [Description("No Updates")]
        No_Updates,

        Lazer
        //Stable40,
        //Beta40,
        //Stable
    }
}
