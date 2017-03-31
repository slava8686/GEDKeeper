﻿/*
 *  "GEDKeeper", the personal genealogical database editor.
 *  Copyright (C) 2009-2017 by Sergey V. Zhdanovskih.
 *
 *  This file is part of "GEDKeeper".
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Drawing;

using GKCommon.GEDCOM;
using GKCore.Interfaces;
using GKCore.Options;
using GKCore.Types;

namespace GKUI.Charts
{
    public interface ITreeChartBox
    {
        void SetScale(float value);
        void GenChart(GEDCOMIndividualRecord iRec, TreeChartKind kind, bool rootCenter);
        void RenderStatic(BackgroundMode background, bool centered);
        void SetRenderer(TreeChartRenderer renderer);
        IBaseWindow Base { get; set; }
        int DepthLimit { get; set; }
        int Height { get; set; }
        Size ImageSize { get; }
        TreeChartOptions Options { get; set; }
        ShieldState ShieldState { get; set; }
        int Width { get; set; }
    }
}
