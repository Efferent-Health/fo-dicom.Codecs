﻿using System;
using System.Linq;
using System.Reflection;

using FellowOakDicom.Imaging.Codec;

namespace FellowOakDicom.Imaging.NativeCodec
{
     /// <summary>
    /// Implementation of <see cref="TranscoderManager"/> for Cross Platform applications.
    /// </summary>
    public sealed class NativeTranscoderManager : TranscoderManager
    {
        public static int MemoryBufferThreshold = 1024 * 1024;

        #region CONSTRUCTORS

        /// <summary>
        /// Initializes an instance of <see cref="NativeTranscoderManager"/>.
        /// </summary>
        public NativeTranscoderManager()
        {
            LoadCodecs(null, null);
        }
        
        #endregion

        #region METHODS

        /// <summary>
        /// Implementation of method to load codecs from assembly(ies) at the specified <paramref name="path"/> and 
        /// with the specified <paramref name="search"/> pattern.
        /// </summary>
        /// <param name="path">Directory path to codec assemblies.</param>
        /// <param name="search">Search pattern for codec assemblies.</param>
        public override void LoadCodecs(string path, string search)
        {
            var assembly = typeof(NativeTranscoderManager).GetTypeInfo().Assembly;
            var codecTypes = assembly.DefinedTypes.Where(
                        ti => ti.IsClass && !ti.IsAbstract && ti.ImplementedInterfaces.Contains(typeof(IDicomCodec)));

            foreach (var codecType in codecTypes)
            {
                var codec = (IDicomCodec)Activator.CreateInstance(codecType);
                Codecs[codec.TransferSyntax] = codec;
            }
        }

        #endregion
    }
}