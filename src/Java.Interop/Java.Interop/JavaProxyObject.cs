﻿using System;
using System.Runtime.CompilerServices;

namespace Java.Interop {

	[JniTypeSignature (JniTypeName)]
	sealed class JavaProxyObject : JavaObject
	{
		internal const string JniTypeName = "com/xamarin/android/internal/JavaProxyObject";

		static  readonly    JniPeerMembers                                  _members        = new JniPeerMembers (JniTypeName, typeof (JavaProxyObject));
		static  readonly    ConditionalWeakTable<object, JavaProxyObject>   CachedValues    = new ConditionalWeakTable<object, JavaProxyObject> ();

		static JavaProxyObject ()
		{
			_members.JniPeerType.RegisterNativeMethods (
					new JniNativeMethodRegistration ("equals",      "(Ljava/lang/Object;)Z",    (Func<IntPtr, IntPtr, IntPtr, bool>)    _Equals),
					new JniNativeMethodRegistration ("hashCode",    "()I",                      (Func<IntPtr, IntPtr, int>)             _GetHashCode),
					new JniNativeMethodRegistration ("toString",    "()Ljava/lang/String;",     (Func<IntPtr, IntPtr, IntPtr>)          _ToString));
		}

		public override JniPeerMembers JniPeerMembers {
			get {
				return _members;
			}
		}

		JavaProxyObject (object value)
		{
			if (value == null)
				throw new ArgumentNullException ("value");
			Value = value;
		}

		public object Value {get; private set;}

		public override int GetHashCode ()
		{
			return Value.GetHashCode ();
		}

		public override bool Equals (object obj)
		{
			var other = obj as JavaProxyObject;
			if (other != null)
				return object.Equals (Value, other.Value);
			return object.Equals (Value, obj);
		}

		public override string ToString ()
		{
			return Value.ToString ();
		}

		public static JavaProxyObject GetProxy (object value)
		{
			if (value == null)
				return null;

			lock (CachedValues) {
				JavaProxyObject proxy;
				if (CachedValues.TryGetValue (value, out proxy))
					return proxy;
				proxy = new JavaProxyObject (value);
				CachedValues.Add (value, proxy);
				return proxy;
			}
		}

		static bool _Equals (IntPtr jnienv, IntPtr n_self, IntPtr n_value)
		{
			var self    = (JavaProxyObject) JniEnvironment.Runtime.ValueManager.PeekObject (new JniObjectReference (n_self));
			var r_value = new JniObjectReference (n_value);
			var value   = JniEnvironment.Runtime.ValueManager.GetValue (ref r_value, JniObjectReferenceOptions.Copy);
			return self.Equals (value);
		}

		static int _GetHashCode (IntPtr jnienv, IntPtr n_self)
		{
			var self = (JavaProxyObject) JniEnvironment.Runtime.ValueManager.PeekObject (new JniObjectReference (n_self));
			return self.GetHashCode ();
		}

		static IntPtr _ToString (IntPtr jnienv, IntPtr n_self)
		{
			var self    = (JavaProxyObject) JniEnvironment.Runtime.ValueManager.PeekObject (new JniObjectReference (n_self));
			var s       = self.ToString ();
			var r       = JniEnvironment.Strings.NewString (s);
			try {
				return JniEnvironment.References.NewReturnToJniRef (r);
			} finally {
				JniObjectReference.Dispose (ref r);
			}
		}
	}
}

