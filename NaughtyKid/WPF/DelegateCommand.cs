using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BearChildren.WPF
{
    public class DelegateCommand :ICommand
    {
        private readonly Action<object> _execute;

        private readonly Predicate<object> _canExecute;

        public DelegateCommand(Action<object> execute)
            : this(execute, null)
        {
            
        }

        private DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if(execute==null)throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }
        /// <summary>
        /// 定义确定此命令是否可在其当前状态下执行的方法
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }
        /// <summary>
        /// 定义在调用此命令时要调用的方法
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }
        /// <summary>
        /// 当出现影响是否应执行该命令的更改时发生
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
