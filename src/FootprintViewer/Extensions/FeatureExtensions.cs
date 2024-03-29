﻿using Mapsui;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FootprintViewer
{
    internal static class FeatureExtensions
    {
        public static string ToDisplayText(this IFeature feature)
        {
            if (feature == null)
            {
                return "no feature";
            }

            if (!feature.Fields.Any())
            {
                return "feature with no attributes";
            }

            var result = new StringBuilder();
            foreach (var field in feature.Fields)
            {
                result.Append($"{field}:{feature[field]}");
            }

            return result.ToString();
        }

        //public static string ToDisplayText(this IFeature feature)
        //{
        //    var result = new StringBuilder();
        //    foreach (var field in feature.Fields)
        //    {
        //        result.Append($"{field}:{feature[field]}");
        //    }

        //    return result.ToString();
        //}

        public static string ToDisplayText(this IEnumerable<KeyValuePair<string, IEnumerable<IFeature>>> featureInfos)
        {
            var result = new StringBuilder();

            foreach (var layer in featureInfos)
            {
                result.Append(layer.Key);
                result.Append(Environment.NewLine);
                foreach (var feature in layer.Value)
                {
                    result.Append(feature.ToDisplayText());
                }
                result.Append(Environment.NewLine);
            }
            return result.ToString();
        }
    }
}