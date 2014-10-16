using System;
using System.Collections;

public class Do : Atom {

	public Action f = Fn.DoNothing;

	public Do () {}

	public Do (Action f)
	{
		this.f = f;
	}

	public Do (Action f, Quantum prev) : base (prev)
	{
		this.f = f;
	}

	public override Atom copyAtom {
		get {
			return new Do (f, prev);
		}
	}

	internal override IEnumerable GetEnumerable ()
	{
		foreach (var _ in Previous()) yield return _;

		try {
			f ();
		} catch (Exception e) {
			ex = e;
		}

		yield return null;
	}

	public static Do _ (Action f) {
		return new Do (f);
	}

	public static Do<A> _<A> (Func<A> f) {
		return new Do<A> (f);
	}

}

public class Do<A> : Chain<A> {

	public Func<A> f;

	public Do (Func<A> f)
	{
		this.f = f;
	}

	public Do (Func<A> f, Quantum prev) : base (prev)
	{
		this.f = f;
	}

	internal override IEnumerable GetEnumerable ()
	{
		foreach (var _ in Previous()) {yield return _;}

		A a = default (A);

		try
		{
			a = f();
		}
		catch (Exception e)
		{
			ex = e;
		}

		yield return a;
	}
	

	public override Chain<A> copyChain {
		get {
			return new Do<A> (f, prev);
		}
	}



}
