export interface IGroupByResult<TModel> {
  key: string | number;
  label?: string;
  items: TModel[];
}

export const groupBy = <TModel>(
  array: TModel[],
  keySelector: (item: TModel) => string | number,
  labelSelector?: (item: TModel) => string,
): IGroupByResult<TModel>[] => {
  const groups: IGroupByResult<TModel>[] = [];

  array.forEach((item) => {
    const key = keySelector(item);
    let group = groups.find((g) => g.key === key);
    if (!group) {
      group = { key, label: labelSelector ? labelSelector(item) : undefined, items: [] };
      groups.push(group);
    }
    group.items.push(item);
  });

  return groups;
};
