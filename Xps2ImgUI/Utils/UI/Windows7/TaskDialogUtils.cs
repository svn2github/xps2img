using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Microsoft.WindowsAPICodePack.Dialogs;

// ReSharper disable CheckNamespace

namespace Windows7.Dialogs
{
    public class TaskDialogCommandInfo
    {
        public TaskDialogCommandInfo(TaskDialogResult taskDialogResult, string text, string instruction = null)
        {
            Text = text;
            TaskDialogResult = taskDialogResult;
            Instruction = instruction;
        }

        public readonly string Text;
        public readonly string Instruction;
        public readonly TaskDialogResult TaskDialogResult;
    }

    public static class TaskDialogUtils
    {
        public static DialogResult Show(IntPtr ownerWindowHandle, string caption, string instructionText, string text, TaskDialogStandardIcon icon, params TaskDialogCommandInfo[] taskDialogCommandInfos)
        {
            if (!TaskDialog.IsPlatformSupported)
            {
                return DialogResult.None;
            }

            using (var taskDialog = new TaskDialog
            {
                OwnerWindowHandle = ownerWindowHandle,
                Caption = caption,
                InstructionText = instructionText,
                Text = text,
                Cancelable = true,
                Icon = icon
            })
            {
                var taskDialogCommandLinks = new List<TaskDialogCommandLink>();
                foreach (var taskDialogCommandInfo in taskDialogCommandInfos)
                {
                    var taskDialogCommandLink = new TaskDialogCommandLink(taskDialogCommandInfo.TaskDialogResult.ToString(), taskDialogCommandInfo.Text, taskDialogCommandInfo.Instruction);
                    taskDialogCommandLink.Click += CommandLinkClicked;
                    taskDialog.Controls.Add(taskDialogCommandLink);
                }

                var taskDialogResult = taskDialog.Show();

                foreach (var taskDialogCommandLink in taskDialogCommandLinks)
                {
                    taskDialogCommandLink.Click -= CommandLinkClicked;
                }

                return taskDialogResult == TaskDialogResult.Cancel ? DialogResult.Cancel : DialogResult.OK;
            }
        }

        private static void CommandLinkClicked(object sender, EventArgs e)
        {
            var taskDialogCommandLink = (TaskDialogCommandLink) sender;
            ((TaskDialog)taskDialogCommandLink.HostingDialog).Close((TaskDialogResult)Enum.Parse(typeof(TaskDialogResult), taskDialogCommandLink.Name));
        }
    }
}