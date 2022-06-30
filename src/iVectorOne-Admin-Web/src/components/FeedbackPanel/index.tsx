import classNames from 'classnames';
import React, { useState } from 'react';

const FeedbackPanel: React.FC = () => {
  const [review, setReview] = useState({
    rating: 3,
    comment: '',
  });

  const handleChange = (e: any) => {
    const { name, value } = e.target;
    setReview({
      ...review,
      [name]: value,
    });
  };

  return (
    <div className='grow'>
      {/* Panel body */}
      <div className='p-6 space-y-6'>
        <div>
          <h2 className='text-2xl text-slate-800 font-bold mb-4'>
            Give Feedback
          </h2>
          <div className='text-sm'>
            Our product depends on customer feedback to improve the overall
            experience!
          </div>
        </div>
        {/* Rate */}
        <section>
          <h3 className='text-xl leading-snug text-slate-800 font-bold mb-6'>
            How likely would you recommend us to a friend or colleague?
          </h3>
          <div className='w-full max-w-xl'>
            <div className='relative'>
              <div
                className='absolute left-0 top-1/2 -mt-px w-full h-0.5 bg-slate-200'
                aria-hidden='true'
              ></div>
              <ul className='relative flex justify-between w-full'>
                {Array.from({ length: 5 }, (_, i) => (
                  <li className='flex' key={i}>
                    <button
                      className={classNames(
                        'w-3 h-3 rounded-full bg-white border-2 border-slate-400',
                        {
                          'bg-indigo-500 border-2 border-indigo-500':
                            i + 1 === review.rating,
                        }
                      )}
                      onClick={() => setReview({ ...review, rating: i + 1 })}
                    >
                      <span className='sr-only'>{i}</span>
                    </button>
                  </li>
                ))}
              </ul>
            </div>
            <div className='w-full flex justify-between text-sm text-slate-500 italic mt-3'>
              <div>Not at all</div>
              <div>Extremely likely</div>
            </div>
          </div>
        </section>
        {/* Tell us in words */}
        <section>
          <h3 className='text-xl leading-snug text-slate-800 font-bold mb-5'>
            Tell us in words
          </h3>
          {/* Form */}
          <label className='sr-only' htmlFor='feedback'>
            Leave a feedback
          </label>
          <textarea
            id='comment'
            name='comment'
            className='form-textarea w-full focus:border-slate-300'
            rows={4}
            placeholder='I really enjoy…'
            value={review.comment}
            onChange={handleChange}
            onBlur={handleChange}
          ></textarea>
        </section>
      </div>

      {/* Panel footer */}
      <footer>
        <div className='flex flex-col px-6 py-5 border-t border-slate-200'>
          <div className='flex self-end'>
            <button className='btn border-slate-200 hover:border-slate-300 text-slate-600'>
              Cancel
            </button>
            <button className='btn bg-primary text-white ml-3'>Save</button>
          </div>
        </div>
      </footer>
    </div>
  );
};

export default React.memo(FeedbackPanel);
