using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Microsoft.WindowsAPICodePack.Dialogs;

using Xps2ImgUI.Utils.UI;

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
        public const DialogResult NotSupported = DialogResult.None;

        public static DialogResult Show(IntPtr ownerWindowHandle, string caption, string instructionText, string text, TaskDialogStandardIcon icon, Action<TaskDialog> beforeShow, params TaskDialogCommandInfo[] taskDialogCommandInfos)
        {
            bool footerCheckBoxChecked;
            return Show(ownerWindowHandle, caption, instructionText, text, icon, null, out footerCheckBoxChecked, beforeShow, taskDialogCommandInfos);
        }

        public static DialogResult Show(IntPtr ownerWindowHandle, string caption, string instructionText, string text, TaskDialogStandardIcon icon, string footerCheckBoxText, out bool footerCheckBoxChecked, Action<TaskDialog> beforeShow, params TaskDialogCommandInfo[] taskDialogCommandInfos)
        {
            using (new ModalGuard())
            {
                if (!TaskDialog.IsPlatformSupported)
                {
                    footerCheckBoxChecked = false;
                    return NotSupported;
                }

                using (var taskDialog = new TaskDialog
                {
                    OwnerWindowHandle = ownerWindowHandle,
                    Caption = caption,
                    InstructionText = instructionText,
                    Text = text,
                    Cancelable = true,
                    Icon = icon,
                    FooterCheckBoxText = footerCheckBoxText
                })
                {
                    var taskDialogCommandLinks = new List<TaskDialogCommandLink>();
                    foreach (var taskDialogCommandLink in taskDialogCommandInfos.Select(taskDialogCommandInfo => new TaskDialogCommandLink(taskDialogCommandInfo.TaskDialogResult.ToString(), taskDialogCommandInfo.Text, taskDialogCommandInfo.Instruction)))
                    {
                        taskDialogCommandLink.Click += CommandLinkClicked;
                        taskDialog.Controls.Add(taskDialogCommandLink);
                    }

                    if (beforeShow != null)
                    {
                        beforeShow(taskDialog);
                    }

                    var taskDialogResult = taskDialog.Show();

                    foreach (var taskDialogCommandLink in taskDialogCommandLinks)
                    {
                        taskDialogCommandLink.Click -= CommandLinkClicked;
                    }

                    footerCheckBoxChecked = taskDialog.FooterCheckBoxChecked ?? false;

                    switch(taskDialogResult)
                    {
                        case TaskDialogResult.None:     return DialogResult.None;
                        case TaskDialogResult.Yes:      return DialogResult.Yes;
                        case TaskDialogResult.No:       return DialogResult.No;
                        case TaskDialogResult.Ok:       return DialogResult.OK;
                        case TaskDialogResult.Cancel:   return DialogResult.Cancel;
                        case TaskDialogResult.Close:    return DialogResult.Cancel;
                        case TaskDialogResult.Retry:    return DialogResult.Retry;
                    }

                    #if DEBUG
                        throw new InvalidOperationException("Unknown code: " + taskDialogResult);
                    #else
                        return NotSupported;
                    #endif
                }
            }
        }

        public static void AddExceptionDetails(TaskDialog taskDialog, Exception ex, string showLess, string showMore)
        {
            if (ex == null)
            {
                return;
            }
            taskDialog.ExpansionMode = TaskDialogExpandedDetailsLocation.ExpandFooter;
            taskDialog.DetailsExpandedLabel = showLess;
            taskDialog.DetailsCollapsedLabel = showMore;
            taskDialog.DetailsExpandedText = ex.ToString();
        }
        
        private static void CommandLinkClicked(object sender, EventArgs e)
        {
            var taskDialogCommandLink = (TaskDialogCommandLink) sender;
            ((TaskDialog)taskDialogCommandLink.HostingDialog).Close((TaskDialogResult)Enum.Parse(typeof(TaskDialogResult), taskDialogCommandLink.Name));
        }
    }
}