using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Seq<A> : IEnumerable, Functor<A> {

	IEnumerable e = Fn.Enumerable (Fn.DoNothing);

	public Seq (IEnumerable<A> seq) {
		this.e = seq;
	}

	public Seq (A a) {
		this.e = Fn.Enumerable (a);
	}
	
	public Seq (IEnumerable seq) {
		this.e = seq;
	}

	public IEnumerable ThenWith (Func<A,IEnumerable> f) {
		var enu = GetEnumerator ();
		while (enu.MoveNext())
			yield return enu.Current;

		enu = f ((A)enu.Current).GetEnumerator ();
		while (enu.MoveNext())
			yield return enu.Current;
	}

	public Seq<B> ThenWith<B> (Func<A,Seq<B>> f) {
		return Bind (f);
	}

	public Seq<B> ThenWith<B> (Func<A,B> f) {
		return FMap (f);
	}

	public IEnumerable ThenWith (Action<A> f) {
		return FMap (f.ToFunc());
	}

	public IEnumerator GetEnumerator () {
		return e.GetEnumerator();
	}

	IEnumerable _FMap<B> (Func<A,B> f) {
		var enu = GetEnumerator ();
		while (enu.MoveNext())
			yield return null;
		
		yield return f ((A)enu.Current);
	}

	public Seq<B> FMap<B> (Func<A,B> f) {
		return new Seq<B> (_FMap (f));
	}

	IEnumerable _Bind<B> (Func<A,Seq<B>> f) {
		var enu = GetEnumerator ();
		while (enu.MoveNext())
			yield return enu.Current;

		enu = f ((A)enu.Current).GetEnumerator();
		while (enu.MoveNext())
			yield return enu.Current;
	}

	public Seq<B> Bind<B> (Func<A,Seq<B>> f) {
		return new Seq<B> (_Bind (f));
	}


	Functor<B> Functor<A>.FMap<B> (Func<A, B> f)
	{
		return FMap (f);
	}
}
