using System;
using System.Collections;
using System.Collections.Generic;

public class MapErrors : Atom {

	Func<Exception, Exception> f;

	public MapErrors (Func<Exception, Exception> f)
	{
		this.f = f;
	}

	public MapErrors (Func<Exception, Exception> f, Quantum prev) : base (prev)
	{
		this.f = f;
	}

	public override Atom copyAtom {
		get {
			return new MapErrors (f, prev);
		}
	}

	internal override IEnumerable GetEnumerable ()
	{
		foreach (var _ in Previous()) yield return _;

		brokenQuanta.FMap (a => {
			try {
				a.ex = f (a.ex);
			}
			catch (Exception e) {
				ex = e;
			}
		});

		yield return null;
	}

	public static MapErrors _ (Func<Exception,Exception> f) {
		return new MapErrors (f);
	}

	public static MapErrors _ (Action<Exception> f) {
		return new MapErrors (f.ToFunc());
	}

}
