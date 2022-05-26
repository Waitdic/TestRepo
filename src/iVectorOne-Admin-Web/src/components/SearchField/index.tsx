import { useState, FC, memo, useEffect, Dispatch, SetStateAction } from 'react';
import { useForm, SubmitHandler } from 'react-hook-form';
import { XIcon } from '@heroicons/react/outline';
//
import { ButtonColors, ButtonVariants } from '@/constants';
import { TextField, Button } from '@/components';

type SearchFormData = {
  searchQuery: string;
};

type Props = {
  list: { name: string }[];
  setList: Dispatch<SetStateAction<any[]>>;
};

const SearchField: FC<Props> = ({ list, setList }) => {
  const [show, setShow] = useState<boolean>(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    setFocus,
    getValues,
  } = useForm<SearchFormData>();

  const handleToggleSearchField = () => {
    if (!show) {
      setShow(true);
    } else {
      resetSearchField();
      setShow(false);
    }
  };

  const resetSearchField = () => {
    setList(list);
    reset();
  };

  const onSubmit: SubmitHandler<SearchFormData> = ({ searchQuery: query }) => {
    if (query.length < 3) {
      setShow(false);
      resetSearchField();
      return;
    }

    const updatedList = list.filter(({ name }) =>
      name.toLowerCase().includes(query.toLowerCase())
    );
    setList(updatedList);
  };

  useEffect(() => {
    if (show) setFocus('searchQuery');
  }, [show, setFocus]);

  return (
    <form onSubmit={handleSubmit(onSubmit)} className='flex gap-1'>
      {show && (
        <div className='relative md:w-60'>
          <TextField
            id='searchQuery'
            hasLabel={false}
            {...register('searchQuery', {
              minLength: {
                value: 3,
                message: 'Must be at least 3 characters.',
              },
            })}
            isDirty={errors.searchQuery ? true : false}
            errorMsg={errors.searchQuery?.message}
            placeholder='Search by name...'
          />
          {show && getValues('searchQuery') && !errors.searchQuery && (
            <XIcon
              className='absolute right-2.5 w-3 h-3 cursor-pointer'
              style={{ top: '50%', transform: 'translateY(-50%)' }}
              onClick={resetSearchField}
            />
          )}
        </div>
      )}
      <Button
        type={show ? ButtonVariants.SUBMIT : ButtonVariants.BUTTON}
        text='Search'
        onClick={!show ? handleToggleSearchField : undefined}
        color={ButtonColors.OUTLINE}
      />
    </form>
  );
};

export default memo(SearchField);
