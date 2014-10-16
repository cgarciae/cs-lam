﻿using System;
using System.Collections;

public abstract class Chain<A> : Atom, Monad<A>, IChain<A> {

	public Chain () {}
	public Chain (Quantum prev) : base (prev) {}

	public Map<A,B> MakeMap<B> (Func<A,B> f) {
		return Map<A,B>._ (f);
	}

	public Map<A,A> MakeMap (Action<A> f) {
		return Map<A,A>._ (f);
	}

	public Chain<B> FMap<B> (Func<A,B> f) {
		return new Map<A,B> (f, this).MakeChain();
	} 

	public Chain<A> FMap (Action<A> f) {
		return FMap (f.ToFunc ());
	}
	public Chain<A> Pure (A value) {
		return FMap (_ => value);
	}
	public Chain<B> Bind<B> (Func<A,Chain<B>> f){
		throw new NotImplementedException ();
		return default (Chain<B>);
	} 
	public Chain<A> XMap (Func<Exception,Exception> f){
		throw new NotImplementedException ();
		return default (Chain<A>);
	} 
	public Chain<A> XMap (Action<Exception> f) {
		return XMap (f.ToFunc ());
	}

	public override Atom copyAtom {
		get {
			return copyChain;
		}
	}

	public abstract Chain<A> copyChain { get;}

	public Chain<A> MakeChain () {
		return this;
	}

	Monad<B> Monad<A>.Bind<B> (Func<A, Monad<B>> f)
	{
		return Bind (f as Func<A,Chain<B>>);
	}

	Functor<A> Applicative<A>.Pure (A value)
	{
		return Pure (value);
	}

	Functor<B> Functor<A>.FMap<B> (Func<A, B> f)
	{
		return FMap (f);
	}

	Functor<A> Functor<A>.XMap (Func<Exception, Exception> f)
	{
		return XMap (f);
	}
}

public interface IChain<A> : IQuantum {
	Chain<A> MakeChain ();
}