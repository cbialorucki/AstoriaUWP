using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// Dex.NET - Mario Kosmiskas
/// 
/// Provided under the Apache 2.0 License: http://www.apache.org/licenses/LICENSE-2.0
/// Commercial use requires attribution
/// </summary>
namespace dex.net
{
	public enum AnnotationVisibility {BUILD=0x00, RUNTIME=0x01, SYSTEM=0x02};

	public class Annotation
	{
		public AnnotationVisibility Visibility { get; private set; }
		public EncodedAnnotation Values { get; private set; }

		public Annotation(BinaryReader reader)
		{
			Visibility = (AnnotationVisibility)reader.ReadByte();
			Values = new EncodedAnnotation(reader);
		}
		
		public static List<Annotation> ReadAnnotationSetItem(BinaryReader reader)
		{
			var annotationsCount = reader.ReadUInt32 ();
			var annotations = new List<Annotation> ((int)annotationsCount);

			for (int i=0; i<annotationsCount; i++) {
				var annotationOffset = reader.ReadUInt32 ();
				if (annotationOffset == 0) {
                    // TODO: Insert placeholder annotation
                    //Console.WriteLine ("Skipping annotation");
                    Debug.WriteLine("Skipping annotation");
					continue;
				}

				var oldPosition = reader.BaseStream.Position;
				reader.BaseStream.Position = annotationOffset;
				annotations.Add (new Annotation (reader));
				reader.BaseStream.Position = oldPosition;
			}

			return annotations;
		}

		public static List<Annotation> ReadAnnotations(BinaryReader reader)
		{
			var annotationsOffset = reader.ReadUInt32 ();
			var oldPosition = reader.BaseStream.Position;
			reader.BaseStream.Position = annotationsOffset;

			var annotations = ReadAnnotationSetItem (reader);

			reader.BaseStream.Position = oldPosition;

			return annotations;
		}
	}

	public class AnnotationElement
	{
		public uint NameIdx { get; private set; }
		public EncodedValue Value { get; private set; }

		public AnnotationElement (BinaryReader reader)
		{
			NameIdx = (uint)Leb128.ReadUleb(reader);
			Value = EncodedValue.parse(reader);
		}

		public string GetName(Dex dex)
		{
			return dex.GetString(NameIdx);
		}
	}

	public class EncodedAnnotation : EncodedValue
	{
		internal uint AnnotationType { get; private set; }
		internal AnnotationElement[] Elements;

		public EncodedAnnotation(BinaryReader reader)
		{
			AnnotationType = Leb128.ReadUleb(reader);
			var size = Leb128.ReadUleb(reader);

			Elements = new AnnotationElement[size];

			for (ulong i=0; i<size; i++) {
				Elements[i] = new AnnotationElement(reader);
			}
		}

		public IEnumerable<AnnotationElement> GetAnnotations()
		{
			for (uint i=0; i<Elements.Length; i++) {
				yield return Elements[i];
			}
		}
	}
}