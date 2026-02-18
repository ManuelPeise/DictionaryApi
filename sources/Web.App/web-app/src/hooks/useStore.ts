import React from 'react';
import isEqual from 'lodash/isEqual';

type Reducer<TModel> = React.Reducer<TModel, Partial<TModel>>;

type StoreResult<TModel> = {
  state: TModel;
  dispatch: (partialState: Partial<TModel> | ((prevState: TModel) => Partial<TModel>)) => void;
  subscribeState: (callback: (state: TModel) => TModel) => () => TModel;
  subScribePartialState: (callback: (state: Partial<TModel>) => Partial<TModel>) => Partial<TModel>;
};

const reducerFunction = <TModel>(
  prevState: TModel,
  partialState: Partial<TModel> | ((prevState: TModel) => Partial<TModel>),
): TModel => {
  const newState =
    typeof partialState === 'function'
      ? (partialState as (prevState: TModel) => Partial<TModel>)(prevState)
      : partialState;

  if (newState == null) {
    return prevState;
  }

  // Handle array merging for TModel[]
  if (Array.isArray(prevState) && Array.isArray(newState)) {
    // Replace array with new array
    return newState as unknown as TModel;
  }

  // Handle object merging for TModel
  if (!Array.isArray(prevState) && !Array.isArray(newState)) {
    const merged = { ...prevState, ...newState };
    if (isEqual(prevState, merged)) {
      return prevState;
    }
    return merged as TModel;
  }

  // Fallback: return prevState if types mismatch
  return prevState;
};

export const useStore = <TModel>(initialState: TModel): StoreResult<TModel> => {
  const reducer: Reducer<TModel> = (state, action) => reducerFunction(state, action);

  const [state, dispatch] = React.useReducer(reducer, initialState);

  // Subscribers ref
  const subscribersRef = React.useRef<Set<(state: TModel) => void>>(new Set());

  // Notify all subscribers on state change
  React.useEffect(() => {
    subscribersRef.current.forEach((cb) => cb(state));
  }, [state]);

  const updateState = React.useCallback(
    (partialState: Partial<TModel> | ((prevState: TModel) => Partial<TModel>)) => {
      const newState =
        typeof partialState === 'function'
          ? (partialState as (prevState: TModel) => Partial<TModel>)(state)
          : partialState;
      dispatch(newState);
    },
    [dispatch, state],
  );

  // Subscribe to all state changes
  const subscribeState = React.useCallback(
    (callback: (state: TModel) => TModel) => {
      // Add to subscribers
      const cb = (s: TModel) => callback(s);
      subscribersRef.current.add(cb);
      // Call immediately with current state
      cb(state);
      // Unsubscribe function
      return () => {
        subscribersRef.current.delete(cb);
        return state;
      };
    },
    [state],
  );

  const subScribePartialState = <TSelected>(selector: (state: TModel) => TSelected): TSelected => {
    return selector(state);
  };

  return { state, dispatch: updateState, subscribeState, subScribePartialState };
};
