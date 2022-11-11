import classNames from 'classnames';

export function syntaxHighlight(json: string) {
  if (!json) return '';

  json = json
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;');
  return json.replace(
    /("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g,
    function (match) {
      const rowClassNames = classNames('text-gray-500', {
        'text-blue-500': /:$/.test(match),
      });
      return '<span class="' + rowClassNames + '">' + match + '</span>';
    }
  );
}
