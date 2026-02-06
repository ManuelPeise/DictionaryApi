import React from 'react';

export type StatelessApiProps<TRequestModel> = {
  requestUrl: string;
  method: 'GET' | 'POST';
  body?: TRequestModel;
  params?: Record<string, string>;
  token?: string;
};

export type StatelessApiModel = {
  sendGetRequest: <TResponse, TRequest>(props: StatelessApiProps<TRequest>) => Promise<TResponse>;
  sendPostRequest: <TResponse, TRequest>(props: StatelessApiProps<TRequest>) => Promise<TResponse>;
};

class StatelessApi {
  private buildRequestUrl = (url: string, params?: Record<string, string>): string => {
    if (!params) return url;

    const queryString = new URLSearchParams(params).toString();

    return queryString ? `${url}?${queryString}` : url;
  };

  private addAuthHeader = (headers: HeadersInit, token?: string): HeadersInit => {
    if (token) {
      return {
        ...headers,
        Authorization: `Bearer ${token}`,
      };
    }
    return headers;
  };

  private sendGetRequest = async <TResponse, TRequest>(
    props: StatelessApiProps<TRequest>,
  ): Promise<TResponse> => {
    try {
      const response = await fetch(this.buildRequestUrl(props.requestUrl, props.params), {
        method: 'GET',
        mode: 'cors',
        headers: this.addAuthHeader({ 'Content-Type': 'application/json' }, props.token),
      });

      if (response.ok) {
        const data = (await response.json()) as TResponse;
        return data;
      } else {
        throw new Error(`Request failed with status ${response.status}`);
      }
    } catch (error) {
      console.error('Error in send get request:', error);
      throw error;
    }
  };

  private sendPostRequest = async <TResponse, TRequest>(
    props: StatelessApiProps<TRequest>,
  ): Promise<TResponse> => {
    try {
      const response = await fetch(this.buildRequestUrl(props.requestUrl, props.params), {
        method: 'POST',
        mode: 'cors',
        headers: this.addAuthHeader({ 'Content-Type': 'application/json' }, props.token),
        body: props.body ? JSON.stringify(props.body) : null,
      });

      if (response.ok) {
        const data = (await response.json()) as TResponse;
        return data;
      } else {
        throw new Error(`Request failed with status ${response.status}`);
      }
    } catch (error) {
      console.error('Error in send post request:', error);
      throw error;
    }
  };

  public static createStatelessApi<TResponse, TRequest>(): StatelessApiModel {
    return {
      sendGetRequest: this.prototype.sendGetRequest,
      sendPostRequest: this.prototype.sendPostRequest,
    };
  }
}

export default StatelessApi;
