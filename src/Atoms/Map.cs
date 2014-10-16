using System;
using System.Collections;

public class Map<A,B> : Quantum, IChain<B> {
	
	public Func<A,B> f;
	
	public Map (Func<A, B> f)
	{
		this.f = f;
	}

	public Map (Func<A, B> f, Chain<A> c) : base (c)
	{
		this.f = f;
	}

	public Map<B,C> MakeMap<C> (Func<B,C> f) {
		return Map<B,C>._ (f);
	}
	
	public Map<B,B> MakeMap (Action<B> f) {
		return Map<B,B>._ (f);
	}
	
	internal override IEnumerable GetEnumerable ()
	{
		var enu = Previous ().GetEnumerator ();

		while (enu.MoveNext())
			yield return enu.Current;


		B b = default (B);

		try
		{
			b = f ((A)enu.Current);
		} 
		catch (Exception e) 
		{
			ex = e;
		}

		yield return b;
	}

	public Chain<B> MakeChain () {

		return new Atomize<B> (this, this);
	}
	
	public override Quantum copyQuantum {
		get {
			var copy = new Map<A,B> (f);
			copy.prev = prev;
			
			return copy;
		}
	}

	public static Map<A,B> _ (Func<A,B> f) {
		return new Map<A, B> (f);
	}
	
	public static Map<A,A> _ (Action<A> f) {
		return new Map<A, A> (f.ToFunc());
	}

	public static Chain<B> operator % (Chain<A> c, Map<A,B> m) {
 		


		return c.copyChain.FMap (m.f);

	}
	
}


