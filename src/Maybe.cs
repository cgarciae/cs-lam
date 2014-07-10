using UnityEngine;
using System;
using System.Collections;

public abstract class Maybe <A> : Functor<A> {

	public abstract bool IsNothing {get;}
	public abstract A value {get;}
	public abstract Maybe<B> FMap<B> (Func<A,B> f);
	public abstract Maybe<A> FMap (Action<A> f);

	Functor<B> Functor<A>.FMap<B> (Func<A, B> f)
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

public class TMaybe {}

public static partial class Fn {

	public static Just<A> Just<A> (A a) {
		return new Just<A> (a);
	}
	
	public static Nothing<A> Nothing<A> () {
		return new Nothing<A> ();
	}

	public static Maybe<A> MakeMaybe<A> ( A obj ){
		return obj == null ? new Nothing<A> () as Maybe<A> : new Just<A> (obj) as Maybe<A>;
	}


	// FUNCTOR
	
	//FMap :: (a -> b) -> Maybe a -> Maybe b
	public static Maybe<B> FMap<A,B> (Func<A,B> f, Maybe<A> F) {
		return F.FMap (f);
	}
	
	//FMap :: (a -> void) -> Maybe a -> Maybe a
	public static Maybe<A> FMap<A> (Action<A> f, Maybe<A> F) {
		return F.FMap (f);
	}
	
	//FMap :: (a -> b) -> (Maybe a -> Maybe b)
	public static Func<Maybe<A>,Maybe<B>> FMap<TMaybe,A,B> (Func<A,B> f) {
		return F => F.FMap (f);
	}

	// APPLICATIVE


}
