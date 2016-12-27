using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NaughtyKid.Annotations;

namespace NaughtyKid.BaseClass
{
    public class  PropertyChangedBase:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged<T>(Expression<Func<T>> propertyname)
        {
            if (PropertyChanged == null) return;
            var PropertyName= propertyname.Body as MemberExpression;
            if (PropertyName != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName.Member.Name)); 
            }
        }
    }

   
}
