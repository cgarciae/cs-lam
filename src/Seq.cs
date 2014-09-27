using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Seq<A> : IEnumerable, Monad<A> {

	IEnumerable e = Fn.Enumerate (Fn.DoNothing);

	public Seq (IEnumerable<A> seq) {
		this.e = seq;
	}

	public Seq (A a) {
		this.e = Fn.Enumerate (a);
	}
	
	public Seq (IEnumerable seq) {
		this.e = seq;
	}

	public Seq<B> ThenDo<B> (Func<B> f) {
		return new Seq<B> (Fn.AppendR (f, e));
	}

	public Seq<B> ThenDo<B> (B value) {
		return new Seq<B> (Fn.AppendR (value, e));
	}

	public IEnumerable ThenWith (Func<A,IEnumerable> f) {
		var enu = GetEnumerator ();
		while (enu.MoveNext())
			yield return enu.Current;

		enu = f ((A)enu.Current).GetEnumerator ();
		while (enu.MoveNext())
			yield return enu.Current;
	}

	public IEnumerator GetEnumerator () {
		return e.GetEnumerator();
	}

	IEnumerable _FMap<B> (Func<A,B> f) {
		var enu = GetEnumerator ();
		while (enu.MoveNext())
			yield return enu.Current;
		
		yield return f ((A)enu.Current);
	}

	public Seq<B> FMap<B> (Func<A,B> f) {
		return new Seq<B> (_FMap (f));
	}

	public Seq<A> FMap (Action<A> f) {
		return FMap (f.ToFunc ());
	}

	public Seq<A> FMap (Action f) {
		return FMap (f.ToFunc<A> ());
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

	public Seq<B> Pure<B> (B value) {
		return new Seq<B> (value);
	}


	public Functor<A> Pure (A value)
	{
		return new Seq<A> (value);
	}

	Functor<B> Functor<A>.FMap<B> (Func<A, B> f)
	{
		return FMap (f);
	}

	Monad<B> Monad<A>.Bind<B> (Func<A,Monad<B>> f) {
		return Bind (f as Func<A,Seq<B>>);
	}


}

public static partial class Fn {

	public static Seq<A> Seq<A> (A value) {
		return new Seq<A> (value);
	}

	public static Seq<A> Seq<A> (Func<A> f) {
		return new Seq<A> (Fn.Enumerate(f));
	}

	public static Seq<A> Seq<A> (IEnumerable<A> e) {
		return new Seq<A> (e);
	}

	public static Seq<A> Seq<A> (Func<IEnumerable<A>> f) {
		return new Seq<A> (Fn.Enumerate (f));
	}

	public static Seq<A> Seq<A> (IEnumerable e) {
		return new Seq<A> (e);
	}

	public static Seq<Maybe<B>> FMap2<A,B> (this Seq<Maybe<A>> F, Func<A,B> f) {
		return F.FMap ((Maybe<A> m) => m.FMap (f));
	}

	public static Seq<Maybe<A>> FMap2<A> (this Seq<Maybe<A>> F, Action<A> f) {
		return F.FMap ((Maybe<A> m) => m.FMap (f));
	}

	public static Seq<Maybe<A>> FMap2<A> (this Seq<Maybe<A>> F, Action f) {
		return F.FMap ((Maybe<A> m) => m.FMap (f));
	}

}