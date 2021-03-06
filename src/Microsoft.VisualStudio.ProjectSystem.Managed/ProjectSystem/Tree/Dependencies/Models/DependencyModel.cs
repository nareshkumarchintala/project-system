﻿// Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Immutable;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.ProjectSystem.Properties;
using Microsoft.VisualStudio.ProjectSystem.VS.Tree.Dependencies;

namespace Microsoft.VisualStudio.ProjectSystem.Tree.Dependencies.Models
{
    internal abstract partial class DependencyModel : IDependencyModel
    {
        [Flags]
        private enum DependencyFlags : byte
        {
            Resolved = 1 << 0,
            Implicit = 1 << 1,
            Visible = 1 << 2
        }

        protected DependencyModel(
            string path,
            string originalItemSpec,
            ProjectTreeFlags flags,
            bool isResolved,
            bool isImplicit,
            IImmutableDictionary<string, string>? properties,
            bool isVisible = true)
        {
            Requires.NotNullOrEmpty(path, nameof(path));
            Requires.NotNullOrEmpty(originalItemSpec, nameof(originalItemSpec));

            Path = path;
            OriginalItemSpec = originalItemSpec;
            Properties = properties ?? ImmutableStringDictionary<string>.EmptyOrdinal;
            Caption = path;
            Flags = flags;

            if (Properties.TryGetBoolProperty("Visible", out bool visibleProperty))
            {
                isVisible = visibleProperty;
            }

            DependencyFlags depFlags = 0;
            if (isResolved)
                depFlags |= DependencyFlags.Resolved;
            if (isVisible)
                depFlags |= DependencyFlags.Visible;
            if (isImplicit)
                depFlags |= DependencyFlags.Implicit;
            _flags = depFlags;
        }

        private readonly DependencyFlags _flags;

        public abstract string ProviderType { get; }

        public virtual string Name => Path;
        public string Caption { get; protected set; }
        public string OriginalItemSpec { get; }
        public string Path { get; }
        public virtual string? SchemaName => null;
        public virtual string? SchemaItemType => null;
        string IDependencyModel.Version => throw new NotImplementedException();
        public bool Resolved => (_flags & DependencyFlags.Resolved) != 0;
        bool IDependencyModel.TopLevel => true;
        public bool Implicit => (_flags & DependencyFlags.Implicit) != 0;
        public bool Visible => (_flags & DependencyFlags.Visible) != 0;
        int IDependencyModel.Priority => throw new NotImplementedException();
        public ImageMoniker Icon => IconSet.Icon;
        public ImageMoniker ExpandedIcon => IconSet.ExpandedIcon;
        public ImageMoniker UnresolvedIcon => IconSet.UnresolvedIcon;
        public ImageMoniker UnresolvedExpandedIcon => IconSet.UnresolvedExpandedIcon;
        public IImmutableDictionary<string, string> Properties { get; }
        IImmutableList<string> IDependencyModel.DependencyIDs => throw new NotImplementedException();
        public ProjectTreeFlags Flags { get; }

        public abstract DependencyIconSet IconSet { get; }

        public string Id => OriginalItemSpec;

        public override string ToString() => $"{ProviderType}-{Id}";
    }
}
