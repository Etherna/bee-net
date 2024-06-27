// Copyright 2021-present Etherna SA
// This file is part of Bee.Net.
// 
// Bee.Net is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Bee.Net is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with Bee.Net.
// If not, see <https://www.gnu.org/licenses/>.

namespace Etherna.BeeNet.Models
{
    public enum RedundancyLevel
    {
        /// <summary>
        /// 0%
        /// </summary>
        None = 0,
        
        /// <summary>
        /// 1%
        /// </summary>
        Medium = 1,
        
        /// <summary>
        /// 5%
        /// </summary>
        Strong = 2,
        
        /// <summary>
        /// 10%
        /// </summary>
        Insane = 3,
        
        /// <summary>
        /// 50%
        /// </summary>
        Paranoid = 4
    }
}