using UnityEngine;
using System;
using System.Collections;
using Functional;

public abstract class Maybe <A> : Functor<A> {

	public abstract bool IsNothing {get;}
	public abstract A value {get;}
	public abstract Maybe<B> FMap<B> (Func<A,B> f);
	public abstract Maybe<A> FMap (Func<A,A> f);
	public abstract Maybe<A> FMap (Action<A> f);

	Functor<B> Functor<A>.FMap<B> (Func<A, B> f)
	{
		return FMap (f);
	}

	Functor<A> Functor<A>.FMap (Func<A, A> f)
	{
		return FMap (f);
	}

	Functor<A> Functor<A>.FMap (Action<A> f)
	{
		return FMap (f);
	}
}

public class Just<A> : Maybe<A> {
	
	A val;
	
	public Just (A val) {
		this.val = val;
	}
	
	public override Maybe<B> FMap <B> (Func <A, B> f)
	{
		return new Just<B> (f (val));
	}
	
	public override Maybe<A> FMap (Func<A, A> f)
	{
		val = f (val);
		return this;
	}
	
	public override Maybe<A> FMap (Action<A> f)
	{
		f (val);
		return this;
	}
	
	public override bool IsNothing {
		get {
			return false;
		}
	}
	
	public override A value {
		get {
			return val;
		}
	}
}

public class Nothing<A> : Maybe<A> {

	public override Maybe<B> FMap<B> (Func<A, B> f)
	{
		return new Nothing<B> ();
	}

	public override Maybe<A> FMap (Func<A, A> f)
	{
		return this;
	}

	public override Maybe<A> FMap (Action<A> f)
	{
		return this;
	}

	public override bool IsNothing {
		get {
			return true;
		}
	}

	public override A value {
		get {
			return default (A);
		}
	}
}

public static class Maybe {

	public static Maybe<A> Make<A> ( A obj ){
		return obj == null ? new Nothing<A> () as Maybe<A> : new Just<A> (obj) as Maybe<A>;
	}

	//Maybe.Apply :: (a -> b) -> (a -> Maybe b)
	public static Func<A,Maybe<B>> Apply<A,B> (Func<A,B> f){
		return a => Apply (f, a);
	}
	//Maybe.Apply :: (a -> b) -> a -> Maybe b
	public static Maybe<B> Apply<A,B> (Func<A,B> f, A a){
		return Make (a).FMap (f);
	}

}
