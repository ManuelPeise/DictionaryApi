import React from 'react';

type ComponentInitializationProps<TModel> = {
  isInitialized: boolean;
  props: TModel;
};

export const useComponentInitialization = <TModel>(
  callback: () => Promise<TModel>,
): ComponentInitializationProps<TModel> => {
  const [isInitialized, setIsInitialized] = React.useState<ComponentInitializationProps<TModel>>({
    isInitialized: false,
    props: {} as TModel,
  });

  React.useEffect(() => {
    const initializeComponent = async () => {
      const result = await callback();
      setIsInitialized({ isInitialized: true, props: result });
    };
    initializeComponent();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return isInitialized;
};
