using System;
using System.Linq;
using System.Reflection;

using FellowOakDicom.Imaging.Codec;
using FellowOakDicom.Log;

namespace Dicom.Imaging.NativeCodec
{
    /// <summary>
    /// Implementation of <see cref="TranscoderManager"/>
    /// </summary>
    public sealed class NativeTranscoderManager : TranscoderManager
    {
        private static bool IsLoaded = false;

        /// <summary>
        /// Singleton instance of the <see cref="NativeTranscodeManager"/>.
        /// </summary>
        public static readonly TranscoderManager Instance;

        /// <summary>
        /// Initializes the static fields of <see cref="NativeTranscodeManager"/>.
        /// </summary>
        static NativeTranscoderManager()
        {
            Instance = new NativeTranscoderManager();
        }

        /// <summary>
        /// Initializes an instance of <see cref="NativeTranscodeManager"/>.
        /// </summary>
        public NativeTranscoderManager()
        {
            this.LoadCodecsImpl(null, null);
        }

        /// <summary>
        /// Implementation of method to load codecs from assembly(ies) at the specified <paramref name="path"/> and 
        /// with the specified <paramref name="search"/> pattern.
        /// </summary>
        /// <param name="path">Directory path to codec assemblies.</param>
        /// <param name="search">Search pattern for codec assemblies.</param>
        protected override void LoadCodecsImpl(string path, string search)
        {
            if (IsLoaded)
                return;

            Codecs.Clear();

            var foundAnyCodecs = false;
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();
            var codecTypes = types.Where(t => typeof(IDicomCodec).IsAssignableFrom(t) && !t.IsAbstract);
            var log = LogManager.GetLogger("fo-dicom.Codecs");

            foreach (var codecType in codecTypes)
            {
                foundAnyCodecs = true;
                IDicomCodec codec = (IDicomCodec)Activator.CreateInstance(codecType);
                Codecs[codec.TransferSyntax] = codec;
            }

            if (!foundAnyCodecs)
            {
                log.Warn("No Dicom codecs were found after searching {path}\\{wildcard}", path, search);
            }
            else
            {
                IsLoaded = true;

                var codecNames = string.Join("\n", Codecs.Keys.Select(k => "- " + k.ToString()));
                System.Diagnostics.Debug.WriteLine($"Codecs found:\n{codecNames}");
            }
        }
    }
}
