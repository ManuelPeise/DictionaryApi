import React from 'react';
import StatelessApi from './statelessApi';
import { useAuth } from './useAuth';

export type StatefulApiProps<TRequestModel> = {
  requestUrl: string;
  method: 'GET' | 'POST';
  params?: Record<string, string>;
};

export type StatefullApiResult<TRequestModel, TResponseModel> = {
  data: TResponseModel | null;
  error: Error | null;
  isLoading: boolean;
  sendGetRequest: (options?: StatefulApiProps<TRequestModel>) => Promise<void>;
  sendPostRequest: (
    model: TRequestModel,
    options?: StatefulApiProps<TRequestModel>,
  ) => Promise<void>;
};

const useStatefullApi = <TRequestModel, TResponseModel>(
  props: StatefulApiProps<TRequestModel>,
): StatefullApiResult<TRequestModel, TResponseModel> => {
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

  const addAuthHeader = (headers: HeadersInit): HeadersInit => {
    const tokens = getTokens();

    if (tokens?.accessToken) {
      return {
        ...headers,
        Authorization: `Bearer ${tokens.accessToken}`,
      };
    }
    return headers;
  };

  const sendGetRequest = async (options?: StatefulApiProps<TRequestModel>): Promise<void> => {
    try {
      if (options) {
        propsRef.current = { ...propsRef.current, ...options };
      }
      setIsLoading(true);

      const response = await fetch(
        buildRequestUrl(propsRef.current.requestUrl, propsRef.current.params),
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
  };

  const sendPostRequest = async (
    model: TRequestModel,
    options?: StatefulApiProps<TRequestModel>,
  ): Promise<void> => {
    try {
      if (options) {
        propsRef.current = { ...propsRef.current, ...options };
      }
      setIsLoading(true);

      const response = await fetch(
        buildRequestUrl(propsRef.current.requestUrl, propsRef.current.params),
        {
          method: 'POST',
          body: JSON.stringify(model),
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
  };

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
  }, [propsRef.current]);

  return {
    data,
    error,
    isLoading,
    sendGetRequest,
    sendPostRequest,
  };
};

export const useApi = {
  createStatelessApi: StatelessApi.createStatelessApi,
  useStatefullApi,
};
