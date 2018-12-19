using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Shell;
using ORMSolutions.ORMArchitect.Core.ObjectModel;
using ORMSolutions.ORMArchitect.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace ORMSolutions.ORMArchitect.Core.Shell
{
    // We need to make the VSCommandSet part of the ORMDesignerDocData class in order to get access to the protected classes...
    public partial class ORMDesignerDocData
    {
        /// <summary>
		/// Create a command set for Visual Studio base windows and menus. Should be called
		/// once when the package loads
		/// </summary>
		/// <param name="serviceProvider">IServiceProvider</param>
		/// <returns></returns>
		public static CommandSet CreateVisualStudioCommandSet(IServiceProvider serviceProvider)
        {
            return new VSCommandSet(serviceProvider);
        }
        /// <summary>
        /// Commands that attach to the Visual Studio base editor
        /// </summary>
        [CLSCompliant(false)]
        protected class VSCommandSet : CommandSet, IEnumerable<MenuCommand>, IDisposable
        {
            // Variables
            private bool isDisposed = false;

            // Properties
            /// <summary>
            /// List of commands in the commandset.
            /// </summary>
            public List<MenuCommand> CommandSet { get; } = new List<MenuCommand>();

            // Constructors
            /// <summary>
            /// Creates the VSCommandSet
            /// </summary>
            /// <param name="serviceProvider"></param>
            public VSCommandSet(IServiceProvider serviceProvider)
                : base(serviceProvider)
            {
                this.CommandSet.Add(new DynamicStatusMenuCommand(new EventHandler(OnStatusRenameImpliedFactTypeSignatures), new EventHandler(OnMenuRenameImpliedFactTypeSignatures), VSCommandSetCommandIds.RenameImpliedFactTypeSignatures));
            }

            // Methods
            /// <summary>
            /// See <see cref="CommandSet.GetMenuCommands"/>.
            /// </summary>
            protected override IList<MenuCommand> GetMenuCommands()
            {
                return this.CommandSet;
            }
            /// <summary>
            /// Called to remove a set of commands. This should be called
            /// by Dispose.
            /// </summary>
            /// <param name="commands">Commands to add</param>
            protected virtual void RemoveCommands(IList<MenuCommand> commands)
            {
                if (base.MenuService != null)
                {
                    foreach (MenuCommand menuCommand in commands)
                    {
                        base.MenuService.RemoveCommand(menuCommand);
                    }
                }
            }
            #region RenameImpliedFactTypeSignatures
            private void OnStatusRenameImpliedFactTypeSignatures(object sender, EventArgs e)
            {
                if (!(sender is MenuCommand command)) return;

                // See if there are any readings that we can fix
                bool canFixErrors = this.GetRenameableReadings().Count > 0;
                command.Enabled = canFixErrors;
                command.Visible = canFixErrors;
            }
            private void OnMenuRenameImpliedFactTypeSignatures(object sender, EventArgs e)
            {
                if (this.MonitorSelection.CurrentDocument is ORMDesignerDocData docData)
                {
                    Store store = docData.Store;
                    using (Transaction t = store.TransactionManager.BeginTransaction(ResourceStrings.RenameImpliedFactTypeSignaturesTransactionName))
                    {
                        // Rename the readings
                        foreach (Reading r in this.GetRenameableReadings())
                        {
                            // There are rules that will automatically rename the fact types and update the reading's signature
                            // The inverse reading will not be the primay for the fact type, so inverse = !r.IsPrimaryForFactType
                            r.Text = Reading.DetermineImplicitFactTypePredicateReading(r.ReadingOrder.FactType, !r.IsPrimaryForFactType);
                        }

                        if (t.HasPendingChanges)
                        {
                            t.Commit();
                        }
                    }
                }
            }
            private List<Reading> GetRenameableReadings()
            {
                List<Reading> results = new List<Reading>();

                if ((this.MonitorSelection.CurrentDocument is ORMDesignerDocData docData)
                    && (docData.TaskProvider is ORMTaskProvider taskProvider))
                {
                    foreach (IORMToolTaskItem i in taskProvider.Tasks)
                    {
                        if (i.ElementLocator is DuplicateReadingSignatureError error)
                        {
                            foreach (Reading r in error.ReadingCollection)
                            {
                                if (r.ReadingOrder?.FactType?.ImpliedByObjectification != null)
                                {
                                    if (!results.Contains(r))
                                    {
                                        results.Add(r);
                                    }
                                }
                            }
                        }
                    }
                }

                return results;
            }
            #endregion

            // Interfaces
            #region IEnumerable<MenuCommand> Implementation
            /// <summary>
            /// Enumerate the current menu commands
            /// </summary>
            public IEnumerator<MenuCommand> GetEnumerator()
            {
                return this.CommandSet.GetEnumerator();
            }
            IEnumerator<MenuCommand> IEnumerable<MenuCommand>.GetEnumerator()
            {
                return this.GetEnumerator();
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
            #endregion // IEnumerable<MenuCommand> Implementation
            #region IDisposable Implementation
            // See: https://docs.microsoft.com/en-us/dotnet/api/system.idisposable
            /// <summary>
            /// Disposes of the VSCommandSet's resources
            /// </summary>
            // Do not make this method virtual.
            // A derived class should not be able to override this method.
            public void Dispose()
            {
                Dispose(true);
                // This object will be cleaned up by the Dispose method.
                // Therefore, you should call GC.SupressFinalize to
                // take this object off the finalization queue
                // and prevent finalization code for this object
                // from executing a second time.
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Disposes of the VSCommandSet's resources
            /// </summary>
            /// <param name="disposing"></param>
            // Dispose(bool disposing) executes in two distinct scenarios.
            // If disposing equals true, the method has been called directly
            // or indirectly by a user's code. Managed and unmanaged resources
            // can be disposed.
            // If disposing equals false, the method has been called by the
            // runtime from inside the finalizer and you should not reference
            // other objects. Only unmanaged resources can be disposed.
            protected virtual void Dispose(bool disposing)
            {
                // Check to see if Dispose has already been called.
                if (!this.isDisposed)
                {
                    // If disposing equals true, dispose all managed
                    // and unmanaged resources.
                    if (disposing)
                    {
                        // Dispose managed resources.
                        this.RemoveCommands(this.CommandSet);
                    }

                    // Call the appropriate methods to clean up
                    // unmanaged resources here.
                    // If disposing is false,
                    // only the following code is executed.


                    // Note disposing has been done.
                    this.isDisposed = true;
                }
            }

            /// <summary>
            /// Deconstructor of VSCommandSet.
            /// </summary>
            // Use C# destructor syntax for finalization code.
            // This destructor will run only if the Dispose method
            // does not get called.
            // It gives your base class the opportunity to finalize.
            // Do not provide destructors in types derived from this class.
            ~VSCommandSet()
            {
                // Do not re-create Dispose clean-up code here.
                // Calling Dispose(false) is optimal in terms of
                // readability and maintainability.
                Dispose(false);
            }
            #endregion
        }

        /// <summary>
        /// CommandIDs for the Visual Studio Commands.
        /// </summary>
        [Guid("C61E6D02-2B5F-475D-804E-60F4F155E5E0")] // keep in sync with PkgCmd.vsct
        protected static class VSCommandSetCommandIds
        {
            /// <summary>
            /// The global identifier for the command set used by the Visual Studio Commands.
            /// </summary>
            private static readonly Guid guidVSCommandSetCommandIds = typeof(VSCommandSetCommandIds).GUID;
            /// <summary>
            /// A command to rename implied fact type signatures -- In Error List Context Menu
            /// </summary>
            public static readonly CommandID RenameImpliedFactTypeSignatures = new CommandID(guidVSCommandSetCommandIds, cmdIdRenameImpliedFactTypeSignatures);
            #region cmdIds
            // IMPORTANT: keep these constants in sync with PkgCmd.vsct

            /// <summary>
            /// A command to rename implied fact type signatures -- In Error List Context Menu
            /// </summary>
            private const int cmdIdRenameImpliedFactTypeSignatures = 0x2000;
            #endregion
        }
    }
}
