﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using NaughtyKidMVVM.Messaging;
using GalaSoft.MvvmLight.Messaging;

namespace NaughtyKidMVVM
{
    public abstract class ViewModelBase : ObservableObject, ICleanup
    {
        private static bool? _isInDesignMode;
        private IMessenger _messengerInstance;

        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        // ReSharper disable PublicConstructorInAbstractClass
        // Must be public to allow for serialization.
        public ViewModelBase()
        // ReSharper restore PublicConstructorInAbstractClass
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        /// <param name="messenger">An instance of a <see cref="Messenger" />
        /// used to broadcast messages to other objects. If null, this class
        /// will attempt to broadcast using the Messenger's default
        /// instance.</param>
        // ReSharper disable PublicConstructorInAbstractClass
        public ViewModelBase(IMessenger messenger)
        // ReSharper restore PublicConstructorInAbstractClass
        {
            MessengerInstance = messenger;
        }

        /// <summary>
        /// Gets a value indicating whether the control is in design mode
        /// (running under Blend or Visual Studio).
        /// </summary>
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "Non static member needed for data binding")]
        public bool IsInDesignMode
        {
            get
            {
                return IsInDesignModeStatic;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the control is in design mode
        /// (running in Blend or Visual Studio).
        /// </summary>
        [SuppressMessage(
            "Microsoft.Security",
            "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
            Justification = "The security risk here is neglectible.")]
        public static bool IsInDesignModeStatic
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                {
#if PORTABLE
                    _isInDesignMode = IsInDesignModePortable();
#elif SILVERLIGHT
                    _isInDesignMode = DesignerProperties.IsInDesignTool;
#elif NETFX_CORE
                    _isInDesignMode = DesignMode.DesignModeEnabled;
#elif XAMARIN
                    _isInDesignMode = false;
#else
                    var prop = DesignerProperties.IsInDesignModeProperty;
                    _isInDesignMode
                        = (bool)DependencyPropertyDescriptor
                                        .FromProperty(prop, typeof(FrameworkElement))
                                        .Metadata.DefaultValue;
#endif
                }

                return _isInDesignMode.Value;
            }
        }

#if PORTABLE
        private static bool IsInDesignModePortable()
        {
            // As a portable lib, we need see what framework we're runnign on
            // and use reflection to get the designer value

            var platform = DesignerLibrary.DetectedDesignerLibrary;

            if (platform == DesignerPlatformLibrary.WinRt)
            {
                return IsInDesignModeMetro();
            }

            if(platform == DesignerPlatformLibrary.Silverlight)
            {
                var desMode = IsInDesignModeSilverlight();
                if (!desMode)
                {
                    desMode = IsInDesignModeNet(); // hard to tell these apart in the designer
                }

                return desMode;
            }

            if (platform == DesignerPlatformLibrary.Net)
            {
                return IsInDesignModeNet();
            }

            return false;
        }

        private static bool IsInDesignModeSilverlight()
        {
            try
            {
                var dm = Type.GetType("System.ComponentModel.DesignerProperties, System.Windows, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e");

                if (dm == null)
                {
                    return false;
                }

                var dme = dm.GetTypeInfo().GetDeclaredProperty("IsInDesignTool");

                if (dme == null)
                {
                    return false;
                }

                return (bool)dme.GetValue(null, null);    
            }
            catch
            {
                return false;
            }
        }

        private static bool IsInDesignModeMetro()
        {
            try
            {
                var dm = Type.GetType("Windows.ApplicationModel.DesignMode, Windows, ContentType=WindowsRuntime");

                var dme = dm.GetTypeInfo().GetDeclaredProperty("DesignModeEnabled");
                return (bool)dme.GetValue(null, null);
            }
            catch
            {
                return false;
            }
        }

        private static bool IsInDesignModeNet()
        {
            try
            {
                var dm =
                    Type.GetType(
                        "System.ComponentModel.DesignerProperties, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");

                if (dm == null)
                {
                    return false;
                }

                var dmp = dm.GetTypeInfo().GetDeclaredField("IsInDesignModeProperty").GetValue(null);

                var dpd =
                    Type.GetType(
                        "System.ComponentModel.DependencyPropertyDescriptor, WindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                var typeFe =
                    Type.GetType("System.Windows.FrameworkElement, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");

                if (dpd == null
                    || typeFe == null)
                {
                    return false;
                }

                var fromPropertys = dpd
                    .GetTypeInfo()
                    .GetDeclaredMethods("FromProperty")
                    .ToList();

                if (fromPropertys == null
                    || fromPropertys.Count == 0)
                {
                    return false;
                }

                var fromProperty = fromPropertys
                    .FirstOrDefault(mi => mi.IsPublic && mi.IsStatic && mi.GetParameters().Length == 2);

                if (fromProperty == null)
                {
                    return false;
                }

                var descriptor = fromProperty.Invoke(null, new[] {dmp, typeFe});

                if (descriptor == null)
                {
                    return false;
                }

                var metaProp = dpd.GetTypeInfo().GetDeclaredProperty("Metadata");

                if (metaProp == null)
                {
                    return false;
                }

                var metadata = metaProp.GetValue(descriptor, null);
                var tPropMeta = Type.GetType("System.Windows.PropertyMetadata, WindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");

                if (metadata == null
                    || tPropMeta == null)
                {
                    return false;
                }

                var dvProp = tPropMeta.GetTypeInfo().GetDeclaredProperty("DefaultValue");

                if (dvProp == null)
                {
                    return false;
                }

                var dv = (bool)dvProp.GetValue(metadata, null);
                return dv;
            }
            catch
            {
                return false;
            }
        }
#endif

        /// <summary>
        /// Gets or sets an instance of a <see cref="IMessenger" /> used to
        /// broadcast messages to other objects. If null, this class will
        /// attempt to broadcast using the Messenger's default instance.
        /// </summary>
        protected IMessenger MessengerInstance
        {
            get
            {
                return _messengerInstance ?? Messenger.Default;
            }
            set
            {
                _messengerInstance = value;
            }
        }

        /// <summary>
        /// Unregisters this instance from the Messenger class.
        /// <para>To cleanup additional resources, override this method, clean
        /// up and then call base.Cleanup().</para>
        /// </summary>
        public virtual void Cleanup()
        {
            MessengerInstance.Unregister(this);
        }

        /// <summary>
        /// Broadcasts a PropertyChangedMessage using either the instance of
        /// the Messenger that was passed to this class (if available) 
        /// or the Messenger's default instance.
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="oldValue">The value of the property before it
        /// changed.</param>
        /// <param name="newValue">The value of the property after it
        /// changed.</param>
        /// <param name="propertyName">The name of the property that
        /// changed.</param>
        protected virtual void Broadcast<T>(T oldValue, T newValue, string propertyName)
        {
            var message = new PropertyChangedMessage<T>(this, oldValue, newValue, propertyName);
            MessengerInstance.Send(message);
        }

        /// <summary>
        /// Raises the PropertyChanged event if needed, and broadcasts a
        /// PropertyChangedMessage using the Messenger instance (or the
        /// static default instance if no Messenger instance is available).
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="propertyName">The name of the property that
        /// changed.</param>
        /// <param name="oldValue">The property's value before the change
        /// occurred.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <param name="broadcast">If true, a PropertyChangedMessage will
        /// be broadcasted. If false, only the event will be raised.</param>
        /// <remarks>If the propertyName parameter
        /// does not correspond to an existing property on the current class, an
        /// exception is thrown in DEBUG configuration only.</remarks>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1026:DefaultParametersShouldNotBeUsed"),
        SuppressMessage(
            "Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This cannot be an event")]
        public virtual void RaisePropertyChanged<T>(
#if CMNATTR
            [CallerMemberName] string propertyName = null, 
#else
            string propertyName,
#endif
            T oldValue = default(T),
            T newValue = default(T),
            bool broadcast = false)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("This method cannot be called with an empty string", "propertyName");
            }

            // ReSharper disable ExplicitCallerInfoArgument
            RaisePropertyChanged(propertyName);
            // ReSharper restore ExplicitCallerInfoArgument

            if (broadcast)
            {
                Broadcast(oldValue, newValue, propertyName);
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event if needed, and broadcasts a
        /// PropertyChangedMessage using the Messenger instance (or the
        /// static default instance if no Messenger instance is available).
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="propertyExpression">An expression identifying the property
        /// that changed.</param>
        /// <param name="oldValue">The property's value before the change
        /// occurred.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <param name="broadcast">If true, a PropertyChangedMessage will
        /// be broadcasted. If false, only the event will be raised.</param>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1030:UseEventsWhereAppropriate",
            Justification = "This cannot be an event")]
        [SuppressMessage(
            "Microsoft.Design",
            "CA1006:GenericMethodsShouldProvideTypeParameter",
            Justification = "This syntax is more convenient than the alternatives.")]
        public virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression, T oldValue, T newValue, bool broadcast)
        {
            RaisePropertyChanged(propertyExpression);

            if (broadcast)
            {
                // Unfortunately I don't see a reliable way to not call GetPropertyName twice.
                var propertyName = GetPropertyName(propertyExpression);
                Broadcast(oldValue, newValue, propertyName);
            }
        }

        /// <summary>
        /// Assigns a new value to the property. Then, raises the
        /// PropertyChanged event if needed, and broadcasts a
        /// PropertyChangedMessage using the Messenger instance (or the
        /// static default instance if no Messenger instance is available). 
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="propertyExpression">An expression identifying the property
        /// that changed.</param>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <param name="broadcast">If true, a PropertyChangedMessage will
        /// be broadcasted. If false, only the event will be raised.</param>
        /// <returns>True if the PropertyChanged event was raised, false otherwise.</returns>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "This syntax is more convenient than the alternatives."),
         SuppressMessage(
            "Microsoft.Design",
            "CA1045:DoNotPassTypesByReference",
            MessageId = "1#")]
        protected bool Set<T>(
            Expression<Func<T>> propertyExpression,
            ref T field,
            T newValue,
            bool broadcast)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

#if !PORTABLE && !SL4
            RaisePropertyChanging(propertyExpression);
#endif
            var oldValue = field;
            field = newValue;
            RaisePropertyChanged(propertyExpression, oldValue, field, broadcast);
            return true;
        }

        /// <summary>
        /// Assigns a new value to the property. Then, raises the
        /// PropertyChanged event if needed, and broadcasts a
        /// PropertyChangedMessage using the Messenger instance (or the
        /// static default instance if no Messenger instance is available). 
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="propertyName">The name of the property that
        /// changed.</param>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <param name="broadcast">If true, a PropertyChangedMessage will
        /// be broadcasted. If false, only the event will be raised.</param>
        /// <returns>True if the PropertyChanged event was raised, false otherwise.</returns>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1026:DefaultParametersShouldNotBeUsed"),
         SuppressMessage(
            "Microsoft.Design",
            "CA1045:DoNotPassTypesByReference",
            MessageId = "1#")]
        protected bool Set<T>(
            string propertyName,
            ref T field,
            T newValue = default(T),
            bool broadcast = false)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

#if !PORTABLE && !SL4
            RaisePropertyChanging(propertyName);
#endif
            var oldValue = field;
            field = newValue;

            // ReSharper disable ExplicitCallerInfoArgument
            RaisePropertyChanged(propertyName, oldValue, field, broadcast);
            // ReSharper restore ExplicitCallerInfoArgument

            return true;
        }

      
#if CMNATTR
        /// <summary>
        /// Assigns a new value to the property. Then, raises the
        /// PropertyChanged event if needed, and broadcasts a
        /// PropertyChangedMessage using the Messenger instance (or the
        /// static default instance if no Messenger instance is available). 
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <param name="broadcast">If true, a PropertyChangedMessage will
        /// be broadcasted. If false, only the event will be raised.</param>
        /// <param name="propertyName">(optional) The name of the property that
        /// changed.</param>
        /// <returns>True if the PropertyChanged event was raised, false otherwise.</returns>
        protected bool Set<T>(
            ref T field,
            T newValue = default(T),
            bool broadcast = false,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

#if !PORTABLE
            RaisePropertyChanging(propertyName);
#endif
            var oldValue = field;
            field = newValue;

            // ReSharper disable ExplicitCallerInfoArgument
            RaisePropertyChanged(propertyName, oldValue, field, broadcast);
            // ReSharper restore ExplicitCallerInfoArgument

            return true;
        }
#endif
    }
}
