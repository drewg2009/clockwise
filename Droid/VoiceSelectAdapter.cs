using System;
using Android;
using Android.Content;
using Android.Views;
using Android.Widget;
using Java.Lang;

public class VoiceSelectAdapter : BaseAdapter
{

    Context context;
    string[] data;
    static LayoutInflater inflater = null;

    public override int Count
    {
        get;
    }

    public VoiceSelectAdapter(Context context, string[] data)
    {
        this.context = context;
        this.data = data;
        inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
    }


    public override Java.Lang.Object GetItem(int position)
    {
        return data[position];
    }

    public override long GetItemId(int position)
    {
        return position;
    }

    public override View GetView(int position, View convertView, ViewGroup parent)
    {
        convertView = View.Inflate(Resource.Layout., null);
        //TextView text = vi.FindViewById<TextView>(Resource.Id.text);
        //text.setText(data[position]);
        return convertView;
    }
}