using System;
using System.Collections;
using System.Collections.Generic;

public class OnLastError : Atom {
	
	Func<Exception, Exception> f;
	
	public OnLastError (Func<Exception, Exception> f)
	{
		this.f = f;
	}

	public OnLastError (Func<Exception, Exception> f, Quantum prev) : base (prev)
	{
		this.f = f;
	}

	public override Atom copyAtom {
		get {
			return new OnLastError (f, prev);
		}
	}
	
	internal override IEnumerable GetEnumerable ()
	{	
		lastBrokenQuantum.FMap (a => {
			try {
				a.ex = f (a.ex);
			}
			catch (Exception e) {
				ex = e;
			}
		});
		
		yield return null;
	}
	
	public static OnLastError _ (Func<Exception,Exception> f) {
		return new OnLastError (f);
	}
	
	public static OnLastError _ (Action<Exception> f) {
		return new OnLastError (f.ToFunc());
	}

	public static OnLastError _ (Action f) {
		return new OnLastError (f.ToFunc<Exception>());
	}
}