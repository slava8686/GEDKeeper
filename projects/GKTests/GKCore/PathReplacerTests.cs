﻿/*
 *  "GEDKeeper", the personal genealogical database editor.
 *  Copyright (C) 2017 by Sergey V. Zhdanovskih.
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
using GKCore;
using NUnit.Framework;

namespace GKTests.GKCore
{
    [TestFixture]
    public class PathReplacerTests
    {
        private PathReplacer fPathReplacer;

        [TestFixtureSetUp]
        public void SetUp()
        {
            fPathReplacer = new PathReplacer();
        }

        [Test]
        public void Test_TryReplacePath()
        {
            string newPath;
            Assert.Throws(typeof(ArgumentNullException), () => { newPath = fPathReplacer.TryReplacePath(null); });

            newPath = fPathReplacer.TryReplacePath(@"C:\TEST\x.yyy");
            Assert.IsNullOrEmpty(newPath);
        }
    }
}
