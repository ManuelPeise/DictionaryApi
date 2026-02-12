import React from 'react';
import StatelessApi from './statelessApi';
import { useAuth } from './useAuth';

export type StatefulApiProps = {
  requestUrl: string;
  method: 'GET' | 'POST';
  params?: Record<string, string>;
};

export type StatefullApiResult<TResponseModel> = {
  data: TResponseModel | null;
  error: Error | null;
  isLoading: boolean;
  sendGetRequest: (options?: StatefulApiProps) => Promise<void>;
  sendPostRequest: (model: any, options: StatefulApiProps) => Promise<void>;
  rebindData: (options?: StatefulApiProps) => Promise<void>;
};

const useStatefullApi = <TResponseModel>(
  props: StatefulApiProps,
): StatefullApiResult<TResponseModel> => {
  const propsRef = React.useRef(props);
  const { getTokens } = useAuth();

  const [data, setData] = React.useState<TResponseModel | null>(null);
  const [error, setError] = React.useState<Error | null>(null);
  const [isLoading, setIsLoading] = React.useState<boolean>(false);

  const buildRequestUrl = (url: string, params?: Record<string, string>): string => {
    if (!params) return url;

    const queryString = new URLSearchParams(params).toString();

    return queryString ? `${url}?${queryString}` : url;
  };

  const addAuthHeader = React.useCallback(
    (headers: HeadersInit): HeadersInit => {
      const tokens = getTokens();

      if (tokens?.accessToken) {
        return {
          ...headers,
          Authorization: `Bearer ${tokens.accessToken}`,
        };
      }
      return headers;
    },
    [getTokens],
  );

  const sendGetRequest = React.useCallback(
    async (options?: StatefulApiProps): Promise<void> => {
      try {
        setIsLoading(true);

        const response = await fetch(
          options
            ? buildRequestUrl(options.requestUrl, options.params)
            : buildRequestUrl(propsRef.current.requestUrl, propsRef.current.params),
          {
            method: 'GET',
            body: null,
            mode: 'cors',
            headers: addAuthHeader({
              'Content-Type': 'application/json',
            }),
          },
        );

        if (response.ok) {
          const responseData = (await response.json()) as TResponseModel;
          setData(responseData);
        } else {
          throw new Error(`Request failed with status ${response.status}`);
        }
      } catch (error) {
        setError(error as Error);
      } finally {
        setIsLoading(false);
      }
    },
    [addAuthHeader],
  );

  const sendPostRequest = React.useCallback(
    async (model: any, options: StatefulApiProps): Promise<void> => {
      try {
        setIsLoading(true);

        const response = await fetch(buildRequestUrl(options.requestUrl, options.params), {
          method: 'POST',
          body: JSON.stringify(model),
          mode: 'cors',
          headers: addAuthHeader({
            'Content-Type': 'application/json',
          }),
        });

        if (response.ok) {
          const responseData = (await response.json()) as TResponseModel;
          setData(responseData);
        } else {
          throw new Error(`Request failed with status ${response.status}`);
        }
      } catch (error) {
        setError(error as Error);
      } finally {
        setIsLoading(false);
      }
    },
    [addAuthHeader],
  );

  const rebindData = React.useCallback(
    async (options?: StatefulApiProps) => {
      await sendGetRequest(options);
    },
    [sendGetRequest],
  );

  React.useEffect(() => {
    if (propsRef.current.method === 'GET') {
      const sendRequest = async () => {
        try {
          await sendGetRequest();
        } catch (error) {
          console.error('Error sending GET request:', error);
        }
      };
      sendRequest();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [propsRef.current]);

  return {
    data,
    error,
    isLoading,
    sendGetRequest,
    sendPostRequest,
    rebindData,
  };
};

export const useApi = {
  createStatelessApi: StatelessApi.createStatelessApi,
  useStatefullApi,
};
