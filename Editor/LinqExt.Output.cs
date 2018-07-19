using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using OfficeOpenXml;

namespace Unity.Editor.LinqExt
{
    public static partial class LinqExt
    {
        ///
        /// <summary></summary>
        ///
        public static void ToExcelFile<T>(this IEnumerable<T> source, string fileName, params string[] columns) where T: UnityEngine.Object
        {
            using (var file = File.OpenWrite(fileName))
            {
                var excelFile = new ExcelPackage(file);
                var excelWorksheet = excelFile.Workbook.Worksheets.Add("Content");

                for (int temp = 0; temp < columns.Length; ++temp)
                    excelWorksheet.SetValue(1, temp, columns[temp]);

                int row = 2;
                foreach (var item in source)
                {
                    var serializedObj = new SerializedObject(item);

                    for(int temp = 0; temp < columns.Length; ++temp)
                    {
                        var property = serializedObj.FindProperty(columns[temp]);
                        if (property == null)
                            continue;

                        excelWorksheet.SetValue(row, temp, property.stringValue);
                    }
                }
            }
        }

        ///
        /// <summary></summary>
        ///
        public static void ToTableUI<T>(this IEnumerable<T> source, params string[] columns) where T : UnityEngine.Object
        {
            // UIElements window
        }


        ///
        /// <summary></summary>
        ///
        public static void ProcessWithProgressBar<T>(this IEnumerable<T> source, Action<T> processor)
        {
            processorTasks.Add(ProcessWithProgressBarCoroutine(source, processor));

            if (processorTasks.Count == 1)
                EditorApplication.update += ProcessWithProgressBarStep;
        }

        /// Editor pseudo-coroutines with progress bar
        private static List<IEnumerator> processorTasks = new List<IEnumerator>();
        private static IEnumerator ProcessWithProgressBarCoroutine<T>(this IEnumerable<T> source, Action<T> processor)
        {
            // We'll need total count for profress bar
            var data = source.ToList();

            for (int temp = 0; temp < data.Count; temp++)
            {
                var value = data[temp];
                processor(value);

                EditorUtility.DisplayCancelableProgressBar("Processing...", "" + temp + "/" + data.Count, (float)temp / data.Count);
                yield return null;
            }
        }
        private static void ProcessWithProgressBarStep()
        {
            for (int temp = 0; temp < processorTasks.Count; ++temp)
            {
                var task = processorTasks[temp];
                if (task.MoveNext())
                    continue;

                processorTasks[temp] = null;
            }
            processorTasks.RemoveAll(null);

            if (processorTasks.Count <= 0)
                EditorApplication.update -= ProcessWithProgressBarStep;
        }
    }
}