﻿// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Dto property collection should be editable", Scope = "namespaceanddescendants", Target = "~N:Etherna.BeeNet.DtoModel")]
[assembly: SuppressMessage("Usage", "CA1707:Remove the underscores from namespace name", Justification = "Version number should containt underscores", Scope = "namespaceanddescendants", Target = "~N:Etherna.BeeNet.Clients")]